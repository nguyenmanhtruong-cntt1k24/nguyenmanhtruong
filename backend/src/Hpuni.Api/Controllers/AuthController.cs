using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

public record LoginInput([Required, MaxLength(40)] string UserName, [Required, MaxLength(128)] string Password);
public record PasswordInput([Required, MaxLength(128)] string CurrentPassword, [Required, MaxLength(128)] string NewPassword);

[ApiController, Route("api/v1/auth"), Authorize]
public class AuthController(HpuniDbContext db, UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn,
    AdminService admin, IAntiforgery antiforgery) : ControllerBase
{
    private string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("csrf"), AllowAnonymous]
    public IActionResult Csrf()
    {
        Response.Headers.CacheControl = "no-store";
        return Ok(new { token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken });
    }

    [HttpPost("login"), AllowAnonymous, EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginInput input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "identity", ct);
        var u = await users.FindByNameAsync(input.UserName);
        if (u == null || !u.Active) return UnauthorizedProblem();
        var check = await signIn.CheckPasswordSignInAsync(u, input.Password, lockoutOnFailure: true);
        if (!check.Succeeded)
        {
            await tx.CommitAsync(ct); // Persist Identity lockout counters.
            return UnauthorizedProblem();
        }
        var now = DateTimeOffset.UtcNow;
        var session = new Hpuni.Domain.LoginSession { UserId = u.Id, CreatedAt = now, ExpiresAt = now.AddHours(8) };
        db.LoginSessions.Add(session);
        admin.Audit(u.Id, "auth.login", u.Id, new { });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        await signIn.SignInWithClaimsAsync(u, new AuthenticationProperties { IsPersistent = false,
            ExpiresUtc = session.ExpiresAt, AllowRefresh = false }, [new Claim("hpuni:session", session.Id)]);
        return Ok(await admin.View(u, ct));
    }

    private ObjectResult UnauthorizedProblem() => Problem(statusCode: 401, title: "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản không khả dụng.",
        extensions: new Dictionary<string, object?> { ["code"] = "LOGIN_FAILED", ["request_id"] = HttpContext.TraceIdentifier });

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        Response.Headers.CacheControl = "no-store";
        var u = await db.Users.AsNoTracking().SingleAsync(x => x.Id == Actor, ct);
        return Ok(await admin.View(u, ct));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var id = User.FindFirstValue("hpuni:session");
        var session = await db.LoginSessions.SingleAsync(x => x.Id == id, ct);
        session.RevokedAt = DateTimeOffset.UtcNow;
        admin.Audit(Actor, "auth.logout", Actor, new { });
        await db.SaveChangesAsync(ct);
        await signIn.SignOutAsync();
        return NoContent();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(PasswordInput input, CancellationToken ct)
    {
        AdminService.ValidatePassword(input.NewPassword);
        if (input.NewPassword == input.CurrentPassword) throw AppException.Invalid("Mật khẩu mới phải khác mật khẩu hiện tại.");
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "identity", ct);
        var u = await db.Users.SingleAsync(x => x.Id == Actor, ct);
        if (!u.Active) throw new AppException(401, "SESSION_EXPIRED", "Phiên đăng nhập hết hiệu lực.");
        AdminService.Check(await users.ChangePasswordAsync(u, input.CurrentPassword, input.NewPassword));
        await admin.RevokeSessions(Actor, ct);
        admin.Audit(Actor, "auth.password-change", Actor, new { });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        await signIn.SignOutAsync();
        return NoContent();
    }
}
