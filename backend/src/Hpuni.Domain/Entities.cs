namespace Hpuni.Domain;

public class Department
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string NormalizedName { get; set; } = "";
    public string Notes { get; set; } = "";
    public bool Active { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}

public class RolePermission
{
    public string RoleId { get; set; } = "";
    public string Permission { get; set; } = "";
}

public class RoleAccountOverride
{
    public string UserId { get; set; } = "";
    public string RoleId { get; set; } = "";
    public string Scope { get; set; } = Scopes.Self;
    public string PermissionsJson { get; set; } = "[]";
}

public class UserDepartmentScope
{
    public string UserId { get; set; } = "";
    public string DepartmentId { get; set; } = "";
}

public class UserPermissionDenial
{
    public string UserId { get; set; } = "";
    public string Permission { get; set; } = "";
}

public class LoginSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
}

public class AuditEvent
{
    public long Id { get; set; }
    public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;
    public string ActorId { get; set; } = "";
    public string Action { get; set; } = "";
    public string TargetId { get; set; } = "";
    public string DetailJson { get; set; } = "{}";
}
