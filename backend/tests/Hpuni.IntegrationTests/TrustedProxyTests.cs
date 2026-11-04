using System.Net;
using Hpuni.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Hpuni.IntegrationTests;

public class TrustedProxyTests
{
    static IConfiguration Config(params (string Key, string? Value)[] values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values.ToDictionary(x => "ReverseProxy:" + x.Key, x => x.Value)).Build();

    [Fact]
    public void DisabledByDefaultAndRejectsEmptyOrWildcardTrust()
    {
        Assert.Null(TrustedProxyConfiguration.Read(Config()));
        Assert.Throws<InvalidOperationException>(() => TrustedProxyConfiguration.Read(Config(("Enabled", "true"))));
        Assert.Throws<InvalidOperationException>(() => TrustedProxyConfiguration.Read(Config(("Enabled", "true"), ("KnownNetworks:0", "0.0.0.0/0"))));
        Assert.Throws<InvalidOperationException>(() => TrustedProxyConfiguration.Read(Config(("Enabled", "true"), ("KnownProxies:0", "*"))));
        Assert.Throws<InvalidOperationException>(() => TrustedProxyConfiguration.Read(Config(("Enabled", "true"), ("KnownProxies:0", "127.0.0.1"), ("ForwardLimit", "0"))));
    }

    [Theory]
    [InlineData("10.0.0.10", "https|203.0.113.8|localhost")]
    [InlineData("10.0.0.99", "http|10.0.0.99|localhost")]
    public async Task OnlyTrustedPeerCanChangeSchemeAndClientIp(string peer, string expected)
    {
        var options = TrustedProxyConfiguration.Read(Config(("Enabled", "true"), ("KnownProxies:0", "10.0.0.10")))!;
        using var host = await new HostBuilder().ConfigureWebHost(web => web.UseTestServer().Configure(app =>
        {
            app.Use((context, next) => { context.Connection.RemoteIpAddress = IPAddress.Parse(peer); return next(context); });
            app.UseForwardedHeaders(options);
            app.Run(context => context.Response.WriteAsync($"{context.Request.Scheme}|{context.Connection.RemoteIpAddress}|{context.Request.Host}"));
        })).StartAsync();
        using var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", "203.0.113.8");
        client.DefaultRequestHeaders.Add("X-Forwarded-Proto", "https");
        client.DefaultRequestHeaders.Add("X-Forwarded-Host", "attacker.invalid");
        Assert.Equal(expected, await client.GetStringAsync("/"));
    }

    [Theory]
    [InlineData("10.2.0.8", "203.0.113.8", "https", "https|203.0.113.8")]
    [InlineData("10.3.0.8", "203.0.113.8", "https", "http|10.3.0.8")]
    [InlineData("10.2.0.8", "198.51.100.7, 203.0.113.8", "http, https", "https|203.0.113.8")]
    [InlineData("10.2.0.8", "203.0.113.8", "http, https", "http|10.2.0.8")]
    public async Task NetworkTrustRespectsHopLimitAndHeaderSymmetry(string peer, string forwardedFor, string proto, string expected)
    {
        var options = TrustedProxyConfiguration.Read(Config(("Enabled", "true"), ("KnownNetworks:0", "10.2.0.0/24")))!;
        using var host = await new HostBuilder().ConfigureWebHost(web => web.UseTestServer().Configure(app =>
        {
            app.Use((context, next) => { context.Connection.RemoteIpAddress = IPAddress.Parse(peer); return next(context); });
            app.UseForwardedHeaders(options);
            app.Run(context => context.Response.WriteAsync($"{context.Request.Scheme}|{context.Connection.RemoteIpAddress}"));
        })).StartAsync();
        using var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", forwardedFor);
        client.DefaultRequestHeaders.Add("X-Forwarded-Proto", proto);
        Assert.Equal(expected, await client.GetStringAsync("/"));
    }
}
