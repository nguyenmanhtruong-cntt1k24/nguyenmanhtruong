using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

// Identity still validates passwords and lockout. SQL compares the original
// values using the same case-insensitive, accent-sensitive collation as indexes.
public class FrontendUserStore(HpuniDbContext db, IdentityErrorDescriber errors)
    : UserStore<ApplicationUser, ApplicationRole, HpuniDbContext>(db, errors)
{
    public override Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed(); cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult<IList<Claim>>([]);
    }

    public override Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Quy?n ???c qu?n l? qua vai tr? v? ph?m vi, kh?ng d?ng Identity claims.");
    public override Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Identity claims kh?ng ???c s? d?ng.");
    public override Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Identity claims kh?ng ???c s? d?ng.");
    public override Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default)
        => Task.FromResult<IList<ApplicationUser>>([]);

    public override Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return Users.SingleOrDefaultAsync(u => u.UserName == normalizedUserName, cancellationToken);
    }

    public override Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return Users.SingleOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    protected override Task<ApplicationRole?> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        => Context.Set<ApplicationRole>().SingleOrDefaultAsync(r => r.Name == normalizedRoleName, cancellationToken);

    public override Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.UserName?.ToUpperInvariant());

    public override Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => Task.FromResult(user.Email?.ToUpperInvariant());
}

public class FrontendRoleStore(HpuniDbContext db, IdentityErrorDescriber errors)
    : RoleStore<ApplicationRole, HpuniDbContext>(db, errors)
{
    public override Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed(); cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(role);
        return Task.FromResult<IList<Claim>>([]);
    }
    public override Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Quy?n ???c qu?n l? qua QuyenVaiTro, kh?ng d?ng Identity claims.");
    public override Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Identity claims kh?ng ???c s? d?ng.");

    public override Task<ApplicationRole?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return Roles.SingleOrDefaultAsync(r => r.Name == normalizedName, cancellationToken);
    }

    public override Task<string?> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken = default)
        => Task.FromResult(role.Name?.ToUpperInvariant());
}
