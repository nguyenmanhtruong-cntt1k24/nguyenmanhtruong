using Hpuni.Domain;
using Xunit;

namespace Hpuni.UnitTests;

public class AccessRulesTests
{
    private static AccessProfile User(string scope = Scopes.All, bool active = true, string[]? denied = null,
        RoleGrant[]? grants = null) => new("u1", "d1", active, scope, ["d2"], denied ?? [],
            grants ?? [new("r1", Scopes.Unit, ["users.manage"])]);

    [Theory]
    [InlineData("all", "d1", "other", true)]
    [InlineData("all", "d2", "other", false)]
    [InlineData("unit", "d1", "other", true)]
    [InlineData("unit", "d2", "u1", false)]
    [InlineData("units", "d2", "u1", false)]
    [InlineData("self", "d1", "u1", true)]
    [InlineData("self", "d1", "other", false)]
    public void IntersectsAccountAndRoleScope(string scope, string department, string owner, bool expected) =>
        Assert.Equal(expected, AccessRules.Allows(User(scope), "users.manage", department, owner));

    [Fact]
    public void NeverCombinesPermissionOfOneRoleWithScopeOfAnother()
    {
        var user = User(grants: [new("r1", Scopes.Self, ["users.manage"]), new("r2", Scopes.All, ["audit.read"])]);
        Assert.False(AccessRules.Allows(user, "users.manage", "d2", "other"));
        Assert.True(AccessRules.Allows(user, "audit.read", "d2", "other"));
    }

    [Fact]
    public void ExplicitDenialAndInactiveAccountWin()
    {
        Assert.False(AccessRules.Allows(User(denied: ["users.manage"]), "users.manage", "d1", "u1"));
        Assert.False(AccessRules.Allows(User(active: false), "users.manage", "d1", "u1"));
        Assert.False(AccessRules.HasPermission(User(), "invented.permission"));
    }

    [Fact]
    public void SelectedUnitsDoNotGrantAllSchool()
    {
        var user = User(Scopes.Units, grants: [new("r", Scopes.All, ["users.manage"])]);
        Assert.True(AccessRules.Allows(user, "users.manage", "d2", "other"));
        Assert.False(AccessRules.Allows(user, "users.manage", "d1", "u1"));
        Assert.False(AccessRules.IsGlobal(user, "users.manage"));
    }
}
