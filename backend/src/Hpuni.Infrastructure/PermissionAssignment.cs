namespace Hpuni.Infrastructure;

// Assigned grants; authorization still applies account scope, status and denials.
public class PermissionAssignment
{
    public string RoleId { get; set; } = "";
    public string RoleName { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Permission { get; set; } = "";
}
