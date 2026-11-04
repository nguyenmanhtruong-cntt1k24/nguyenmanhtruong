using System.Net;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class ResponseHeadersTests(SqlFactory factory) : IClassFixture<SqlFactory>
{
    [Fact]
    public async Task ApiHeadersSurviveSuccessAuthorizationAndApplicationExceptions()
    {
        using var anonymous = factory.Client();
        using var admin = await factory.Login();
        var user = await SqlFactory.Create(admin);
        using var lecturer = await factory.Login(user.UserName);
        foreach (var (client, path, status) in new[]
        {
            (lecturer, "/api/v1/rooms?pageSize=1", HttpStatusCode.OK),
            (anonymous, "/api/v1/rooms", HttpStatusCode.Unauthorized),
            (lecturer, "/api/v1/rooms?page=0", HttpStatusCode.BadRequest),
            (lecturer, "/api/v1/equipment/nonexistent-audit-asset", HttpStatusCode.NotFound)
        })
        {
            using var response = await client.GetAsync(path);
            Assert.Equal(status, response.StatusCode);
            Assert.Equal("nosniff", Assert.Single(response.Headers.GetValues("X-Content-Type-Options")));
            Assert.Equal("same-origin", Assert.Single(response.Headers.GetValues("Referrer-Policy")));
            Assert.True(response.Headers.CacheControl?.NoStore);
        }
    }
}
