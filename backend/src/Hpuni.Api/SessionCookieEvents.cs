using System.Security.Claims;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api;

public class SessionCookieEvents(HpuniDbContext db) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessionId = context.Principal?.FindFirstValue("hpuni:session");
        var now = DateTimeOffset.UtcNow;
        var valid = userId != null && sessionId != null && await (
            from s in db.LoginSessions.AsNoTracking()
            join u in db.Users.AsNoTracking() on s.UserId equals u.Id
            where s.Id == sessionId && u.Id == userId && u.Active && s.RevokedAt == null && s.ExpiresAt > now
            select s.Id).AnyAsync(context.HttpContext.RequestAborted);
        if (!valid) context.RejectPrincipal();
    }

    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context) =>
        Error(context, 401, "SESSION_REQUIRED", "Vui lòng đăng nhập.");
    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context) =>
        Error(context, 403, "FORBIDDEN", "Bạn không có quyền truy cập.");

    private static Task Error(RedirectContext<CookieAuthenticationOptions> c, int status, string code, string message) =>
        Results.Problem(statusCode: status, title: message, extensions: new Dictionary<string, object?>
        { ["code"] = code, ["request_id"] = c.HttpContext.TraceIdentifier }).ExecuteAsync(c.HttpContext);
}
