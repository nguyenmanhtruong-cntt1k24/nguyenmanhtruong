using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

namespace Hpuni.Api;

public static class TrustedProxyConfiguration
{
    public static ForwardedHeadersOptions? Read(IConfiguration configuration)
    {
        var section = configuration.GetSection("ReverseProxy");
        if (!section.GetValue<bool>("Enabled")) return null;
        var proxies = section.GetSection("KnownProxies").Get<string[]>() ?? [];
        var networks = section.GetSection("KnownNetworks").Get<string[]>() ?? [];
        if (proxies.Length + networks.Length == 0)
            throw new InvalidOperationException("ReverseProxy cần ít nhất một proxy hoặc mạng tin cậy cụ thể.");
        var limit = section.GetValue<int?>("ForwardLimit") ?? 1;
        if (limit is < 1 or > 5) throw new InvalidOperationException("ReverseProxy:ForwardLimit phải từ 1 đến 5.");
        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            ForwardLimit = limit,
            RequireHeaderSymmetry = true
        };
        options.KnownProxies.Clear();
        options.KnownIPNetworks.Clear();
        foreach (var value in proxies)
        {
            if (!IPAddress.TryParse(value, out var address) || address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
                throw new InvalidOperationException("ReverseProxy:KnownProxies cần địa chỉ IP cụ thể.");
            options.KnownProxies.Add(address);
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                options.KnownProxies.Add(address.MapToIPv6());
        }
        foreach (var value in networks)
        {
            if (!System.Net.IPNetwork.TryParse(value, out var network) || network.PrefixLength == 0)
                throw new InvalidOperationException("ReverseProxy:KnownNetworks cần CIDR cụ thể, không cho phép /0.");
            options.KnownIPNetworks.Add(network);
        }
        return options;
    }
}
