using System.Text.Json;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class AccessService(HpuniDbContext db) : IAccessService
{
    public async Task<AccessProfile> GetProfile(string userId, CancellationToken ct = default)
    {
        var u = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId, ct)
            ?? throw new AppException(401, "SESSION_EXPIRED", "Phiên đăng nhập không còn hiệu lực.");
        var roles = await (from ur in db.UserRoles where ur.UserId == userId
                           join r in db.Roles on ur.RoleId equals r.Id select r).AsNoTracking().ToListAsync(ct);
        var ids = roles.Select(x => x.Id).ToArray();
        var permissions = await db.RolePermissions.AsNoTracking().Where(x => ids.Contains(x.RoleId)).ToListAsync(ct);
        var overrides = await db.RoleAccountOverrides.AsNoTracking().Where(x => x.UserId == userId).ToListAsync(ct);
        var grants = roles.Select(r =>
        {
            var o = overrides.SingleOrDefault(x => x.RoleId == r.Id);
            return new RoleGrant(r.Id, o?.Scope ?? r.Scope, o == null
                ? permissions.Where(x => x.RoleId == r.Id).Select(x => x.Permission).ToArray()
                : JsonSerializer.Deserialize<string[]>(o.PermissionsJson) ?? []);
        }).ToArray();
        return new(u.Id, u.DepartmentId, u.Active, u.Scope,
            await db.UserDepartmentScopes.Where(x => x.UserId == userId).Select(x => x.DepartmentId).ToArrayAsync(ct),
            await db.UserPermissionDenials.Where(x => x.UserId == userId).Select(x => x.Permission).ToArrayAsync(ct), grants);
    }
}
