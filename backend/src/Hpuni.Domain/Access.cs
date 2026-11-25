namespace Hpuni.Domain;

public static class Scopes
{
    public const string All = "all", Unit = "unit", Units = "units", Self = "self";
    public static bool Valid(string scope, bool account = true) =>
        scope is All or Unit or Self || (account && scope == Units);
}

public static class Permissions
{
    public static readonly string[] CoreAdmin = ["users.manage", "roles.manage", "permissions.manage", "audit.read"];
    public static readonly string[] All = [.. CoreAdmin, "departments.manage", "budget.write", "budget.appraise", "budget.approve",
        "transactions.write", "budget.read", "rooms.book", "devices.borrow", "resources.manage", "requests.review",
        "inventory.manage", "logs.write", "logs.review", "reports.read", "reports.export"];
}

public record RoleGrant(string RoleId, string Scope, IReadOnlyCollection<string> Permissions);
public record AccessProfile(string UserId, string DepartmentId, bool Active, string Scope,
    IReadOnlyCollection<string> DepartmentIds, IReadOnlyCollection<string> Denied, IReadOnlyCollection<RoleGrant> Grants);

public static class AccessRules
{
    public static bool HasPermission(AccessProfile user, string permission) => user.Active &&
        Permissions.All.Contains(permission) && !user.Denied.Contains(permission) &&
        user.Grants.Any(g => g.Permissions.Contains(permission));

    public static bool Allows(AccessProfile user, string permission, string? departmentId, string? ownerId) =>
        HasPermission(user, permission) && Match(user, user.Scope, departmentId, ownerId) &&
        user.Grants.Any(g => g.Permissions.Contains(permission) && Match(user, g.Scope, departmentId, ownerId));

    public static bool IsGlobal(AccessProfile user, string permission) => HasPermission(user, permission) &&
        user.Scope == Scopes.All && user.Grants.Any(g => g.Scope == Scopes.All && g.Permissions.Contains(permission));

    private static bool Match(AccessProfile user, string scope, string? departmentId, string? ownerId) => scope switch
    {
        Scopes.All => true,
        Scopes.Unit => departmentId != null && departmentId == user.DepartmentId,
        Scopes.Units => departmentId != null && user.DepartmentIds.Contains(departmentId),
        Scopes.Self => ownerId != null && ownerId == user.UserId,
        _ => false
    };
}
