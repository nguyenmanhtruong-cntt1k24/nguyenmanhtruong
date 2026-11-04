using Hpuni.Domain;

namespace Hpuni.Infrastructure;

public static class AccessQueries
{
    public static IQueryable<ApplicationUser> Users(IQueryable<ApplicationUser> query, AccessProfile profile, string permission)
    {
        if (!AccessRules.HasPermission(profile, permission)) return query.Where(x => false);
        var departments = profile.DepartmentIds.ToArray();
        query = profile.Scope switch
        {
            Scopes.All => query,
            Scopes.Unit => query.Where(x => x.DepartmentId == profile.DepartmentId),
            Scopes.Units => query.Where(x => departments.Contains(x.DepartmentId)),
            Scopes.Self => query.Where(x => x.Id == profile.UserId),
            _ => query.Where(x => false)
        };
        var granting = profile.Grants.Where(x => x.Permissions.Contains(permission)).ToArray();
        var all = granting.Any(x => x.Scope == Scopes.All);
        var unit = granting.Any(x => x.Scope == Scopes.Unit);
        var self = granting.Any(x => x.Scope == Scopes.Self);
        return query.Where(x => all || unit && x.DepartmentId == profile.DepartmentId || self && x.Id == profile.UserId);
    }
}
