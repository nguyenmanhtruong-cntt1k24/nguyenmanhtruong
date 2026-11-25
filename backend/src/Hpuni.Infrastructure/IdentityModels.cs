using Hpuni.Domain;
using Microsoft.AspNetCore.Identity;

namespace Hpuni.Infrastructure;

public class ApplicationUser : IdentityUser
{
    public DateOnly? DateOfBirth { get; set; }
    public string? DepartmentName { get; private set; }
    public string DisplayName { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public string Scope { get; set; } = Scopes.Self;
    public bool Active { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}

public class ApplicationRole : IdentityRole
{
    public string Scope { get; set; } = Scopes.Self;
    public bool BuiltIn { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
