using System.Text.Json;
using System.Text.RegularExpressions;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class AdminService(HpuniDbContext db, UserManager<ApplicationUser> users,
    RoleManager<ApplicationRole> roles, IAccessService access)
{
    public async Task DemandGlobal(string actor, string permission, CancellationToken ct = default)
    {
        if (!AccessRules.IsGlobal(await access.GetProfile(actor, ct), permission)) throw AppException.Forbidden();
    }

    public async Task DemandAdministrationWrite(string actor, CancellationToken ct = default)
    {
        var profile = await access.GetProfile(actor, ct);
        if (!Permissions.CoreAdmin.All(p => AccessRules.IsGlobal(profile, p))) throw AppException.Forbidden();
    }

    public async Task<UserView> View(ApplicationUser u, CancellationToken ct = default)
    {
        var p = await access.GetProfile(u.Id, ct);
        return new(u.Id, u.UserName!, u.Email!, u.DisplayName, u.DepartmentId, u.Active, u.Scope,
            Convert.ToBase64String(u.RowVersion), p.Grants.Select(x => x.RoleId).ToArray(),
            p.DepartmentIds.ToArray(), p.Denied.ToArray(), p.Grants.ToArray(),
            await db.RoleAccountOverrides.AsNoTracking().Where(x=>x.UserId==u.Id).Select(x=>x.RoleId).ToArrayAsync(ct));
    }

    public async Task<RoleView> View(ApplicationRole r, CancellationToken ct = default) =>
        new(r.Id, r.Name!, r.Scope, r.BuiltIn, Convert.ToBase64String(r.RowVersion),
            await db.RolePermissions.Where(x => x.RoleId == r.Id).Select(x => x.Permission).ToArrayAsync(ct));

    public static DepartmentView View(Department d) => new(d.Id, d.Code, d.Name, d.Notes, d.Active,
        Convert.ToBase64String(d.RowVersion));

    public async Task<UserView> CreateUser(string actor, CreateUser input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "identity", ct);
        await DemandGlobal(actor, "users.manage", ct);
        await DemandAdministrationWrite(actor, ct);
        if (!Regex.IsMatch(input.UserName ?? "", "^[a-zA-Z0-9][a-zA-Z0-9._-]{2,39}$"))
            throw AppException.Invalid("Tên đăng nhập cần 3–40 ký tự chữ không dấu, số, dấu chấm, gạch ngang hoặc gạch dưới.");
        await ValidateDepartment(input.DepartmentId, ct);
        await ValidateRoles(input.RoleIds, ct);
        ValidatePassword(input.Password);
        var u = new ApplicationUser { UserName = input.UserName!.Trim(), Email = Required(input.Email, 256),
            DisplayName = Required(input.DisplayName, 150), DepartmentId = input.DepartmentId };
        Check(await users.CreateAsync(u, input.Password));
        foreach (var id in input.RoleIds.Distinct())
            db.UserRoles.Add(new() { UserId = u.Id, RoleId = id });
        Audit(actor, "user.create", u.Id, new { u.UserName, u.Email, u.DisplayName, u.DepartmentId, input.RoleIds });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return await View(u, ct);
    }

    public async Task<UserView> UpdateUser(string actor, string id, UpdateUser input, CancellationToken ct)
    {
        return await MutateUser(actor, id, input.Version, "users.manage", "user.update", async u =>
        {
            await ValidateDepartment(input.DepartmentId, ct);
            await ValidateRoles(input.RoleIds, ct);
            u.Email = Required(input.Email, 256);
            u.DisplayName = Required(input.DisplayName, 150);
            u.DepartmentId = input.DepartmentId;
            var assigned = await db.UserRoles.Where(x => x.UserId == id).ToListAsync(ct);
            foreach (var r in assigned.Where(x => !input.RoleIds.Contains(x.RoleId))) db.UserRoles.Remove(r);
            foreach (var roleId in input.RoleIds.Distinct().Except(assigned.Select(x => x.RoleId)))
                db.UserRoles.Add(new() { UserId = id, RoleId = roleId });
            Check(await users.UpdateAsync(u));
        }, ct);
    }

    public async Task<UserView> Status(string actor, string id, ChangeStatus input, CancellationToken ct) =>
        await MutateUser(actor, id, input.Version, "users.manage", "user.status", async u =>
        {
            if (actor == id && !input.Active) throw AppException.Invalid("Không thể tự khóa tài khoản đang thao tác.");
            if (u.Active == input.Active) throw AppException.Stale();
            u.Active = input.Active;
            if (!input.Active) await RevokeSessions(id, ct);
        }, ct, new { reason = OptionalNote(input.Reason) });

    public async Task<UserView> Scope(string actor, string id, ConfigureScope input, CancellationToken ct) =>
        await MutateUser(actor, id, input.Version, "permissions.manage", "user.scope", async u =>
        {
            if (!Scopes.Valid(input.Scope) || input.DepartmentIds == null || input.Denied == null)
                throw AppException.Invalid("Phạm vi không hợp lệ.");
            ValidatePermissions(input.Denied);
            if (input.Scope == Scopes.Units && input.DepartmentIds.Length == 0)
                throw AppException.Invalid("Cần chọn ít nhất một đơn vị.");
            foreach (var dep in input.DepartmentIds.Distinct()) await ValidateDepartment(dep, ct);
            u.Scope = input.Scope;
            db.UserDepartmentScopes.RemoveRange(await db.UserDepartmentScopes.Where(x => x.UserId == id).ToListAsync(ct));
            db.UserPermissionDenials.RemoveRange(await db.UserPermissionDenials.Where(x => x.UserId == id).ToListAsync(ct));
            // Save removals before adding identical composite keys to the change tracker.
            await db.SaveChangesAsync(ct);
            if (input.Scope == Scopes.Units)
                db.UserDepartmentScopes.AddRange(input.DepartmentIds.Distinct().Select(x => new UserDepartmentScope { UserId = id, DepartmentId = x }));
            db.UserPermissionDenials.AddRange(input.Denied.Distinct().Select(x => new UserPermissionDenial { UserId = id, Permission = x }));
        }, ct);

    public async Task<UserView> Override(string actor, string id, ConfigureOverride input, CancellationToken ct) =>
        await MutateUser(actor, id, input.Version, "permissions.manage", "user.role-override", async u =>
        {
            if (!await db.UserRoles.AnyAsync(x => x.UserId == id && x.RoleId == input.RoleId, ct))
                throw AppException.Invalid("Tài khoản không thuộc vai trò này.");
            var old = await db.RoleAccountOverrides.SingleOrDefaultAsync(x => x.UserId == id && x.RoleId == input.RoleId, ct);
            if (input.Inherit) { if (old != null) db.RoleAccountOverrides.Remove(old); }
            else
            {
                if (!Scopes.Valid(input.Scope, false)) throw AppException.Invalid("Phạm vi vai trò không hợp lệ.");
                ValidatePermissions(input.Permissions);
                if (old == null) { old = new() { UserId = id, RoleId = input.RoleId }; db.RoleAccountOverrides.Add(old); }
                old.Scope = input.Scope;
                old.PermissionsJson = JsonSerializer.Serialize(input.Permissions.Distinct());
            }
            u.ConcurrencyStamp = Guid.NewGuid().ToString();
        }, ct);

    public async Task ResetPassword(string actor, string id, string password, string version, CancellationToken ct)
    {
        ValidatePassword(password);
        await MutateUser(actor, id, version, "users.manage", "user.password-reset", async u =>
        {
            var token = await users.GeneratePasswordResetTokenAsync(u);
            Check(await users.ResetPasswordAsync(u, token, password));
            await RevokeSessions(id, ct);
        }, ct);
    }

    private async Task<UserView> MutateUser(string actor, string id, string version, string permission,
        string action, Func<ApplicationUser, Task> mutate, CancellationToken ct, object? context = null)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "identity", ct);
        await DemandGlobal(actor, permission, ct);
        await DemandAdministrationWrite(actor, ct);
        var actorProfile = await access.GetProfile(actor, ct);
        var protectSelf = Permissions.CoreAdmin.All(p => AccessRules.IsGlobal(actorProfile, p));
        var u = await db.Users.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw AppException.Missing();
        Version(u.RowVersion, version);
        var before = await View(u, ct);
        await mutate(u);
        // Force a parent row update for scope/override changes and their concurrency token.
        u.ConcurrencyStamp = Guid.NewGuid().ToString();
        await db.SaveChangesAsync(ct);
        await ProtectAdministrators(actor, protectSelf, ct);
        var after = await View(u, ct);
        Audit(actor, action, id, new { before, after, context });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return after;
    }

    public async Task<RoleView> SaveRole(string actor, string? id, SaveRole input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "identity", ct);
        await DemandGlobal(actor, "roles.manage", ct);
        await DemandGlobal(actor, "permissions.manage", ct);
        await DemandAdministrationWrite(actor, ct);
        var actorProfile = await access.GetProfile(actor, ct);
        var protectSelf = Permissions.CoreAdmin.All(p => AccessRules.IsGlobal(actorProfile, p));
        if (!Scopes.Valid(input.Scope, false)) throw AppException.Invalid("Phạm vi vai trò không hợp lệ.");
        ValidatePermissions(input.Permissions);
        var r = id == null ? new ApplicationRole() : await db.Roles.SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw AppException.Missing();
        if (id != null) Version(r.RowVersion, input.Version);
        if (r.Id == "admin" && (input.Scope != Scopes.All || Permissions.CoreAdmin.Except(input.Permissions).Any()))
            throw AppException.Invalid("Vai trò quản trị gốc phải giữ quyền cốt lõi và phạm vi toàn trường.");
        var before = id == null ? null : await View(r, ct);
        r.Name = Required(input.Name, 150);
        r.Scope = input.Scope;
        Check(id == null ? await roles.CreateAsync(r) : await roles.UpdateAsync(r));
        db.RolePermissions.RemoveRange(await db.RolePermissions.Where(x => x.RoleId == r.Id).ToListAsync(ct));
        await db.SaveChangesAsync(ct);
        db.RolePermissions.AddRange(input.Permissions.Distinct().Select(x => new RolePermission { RoleId = r.Id, Permission = x }));
        await db.SaveChangesAsync(ct);
        await ProtectAdministrators(actor, protectSelf, ct);
        var after = await View(r, ct);
        Audit(actor, "role.save", r.Id, new { before, after });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return after;
    }

    public async Task<DepartmentView> SaveDepartment(string actor, string? id, SaveDepartment input, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "identity", ct);
        await DemandGlobal(actor, "departments.manage", ct);
        await DemandAdministrationWrite(actor, ct);
        var code = Required(input.Code, 30).ToUpperInvariant();
        if (!Regex.IsMatch(code, "^[A-Z0-9_-]{2,30}$")) throw AppException.Invalid("Mã đơn vị không hợp lệ.");
        var d = id == null ? new Department() : await db.Departments.SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw AppException.Missing();
        if (id != null) Version(d.RowVersion, input.Version);
        var before = id == null ? null : View(d);
        if (!input.Active && (await db.Users.AnyAsync(x => x.DepartmentId == d.Id, ct) ||
            await db.UserDepartmentScopes.AnyAsync(x => x.DepartmentId == d.Id, ct)))
            throw AppException.Invalid("Đơn vị đang liên kết tài khoản hoặc phạm vi; chưa thể ngừng hoạt động.");
        d.Code = code; d.Name = Required(input.Name, 150); d.NormalizedName = d.Name.ToUpperInvariant();
        d.Notes = input.Notes?.Trim() ?? ""; d.Active = input.Active;
        if (d.Notes.Length > 2000) throw AppException.Invalid("Ghi chú tối đa 2000 ký tự.");
        if (id == null) db.Departments.Add(d);
        Audit(actor, "department.save", d.Id, new { before, after = new { d.Code, d.Name, d.Notes, d.Active } });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return View(d);
    }

    private async Task ProtectAdministrators(string actor, bool protectSelf, CancellationToken ct)
    {
        bool FullAdmin(AccessProfile p) => Permissions.CoreAdmin.All(x => AccessRules.IsGlobal(p, x));
        // Only prevent self-removal when the actor was a full administrator before this transaction.
        // All system writers are required to retain core administration for the active operator.
        if (protectSelf && !FullAdmin(await access.GetProfile(actor, ct)))
            throw AppException.Invalid("Không thể tự thu hồi quyền quản trị cốt lõi.");
        var active = await db.Users.AsNoTracking().Where(x => x.Active).Select(x => x.Id).ToListAsync(ct);
        foreach (var id in active)
            if (FullAdmin(await access.GetProfile(id, ct))) return;
        throw AppException.Invalid("Hệ thống phải còn ít nhất một quản trị viên hoạt động.");
    }

    public async Task RevokeSessions(string userId, CancellationToken ct)
    {
        var sessions = await db.LoginSessions.Where(x => x.UserId == userId && x.RevokedAt == null).ToListAsync(ct);
        foreach (var s in sessions) s.RevokedAt = DateTimeOffset.UtcNow;
    }

    private async Task ValidateDepartment(string id, CancellationToken ct)
    {
        if (!await db.Departments.AnyAsync(x => x.Id == id && x.Active, ct)) throw AppException.Invalid("Đơn vị không hợp lệ hoặc đã ngừng hoạt động.");
    }
    private async Task ValidateRoles(string[] ids, CancellationToken ct)
    {
        if (ids == null || ids.Length == 0 || ids.Length > 50 || await db.Roles.CountAsync(x => ids.Contains(x.Id), ct) != ids.Distinct().Count())
            throw AppException.Invalid("Chọn ít nhất một vai trò hợp lệ.");
    }
    public static void ValidatePermissions(string[]? permissions)
    {
        if (permissions == null || permissions.Except(Permissions.All).Any()) throw AppException.Invalid("Quyền chức năng không hợp lệ.");
    }
    public static void Version(byte[] actual, string? expected)
    {
        try { if (expected == null || !actual.SequenceEqual(Convert.FromBase64String(expected))) throw AppException.Stale(); }
        catch (FormatException) { throw AppException.Invalid("Phiên bản dữ liệu không hợp lệ."); }
    }
    public static string Required(string? s, int max)
    {
        s = s?.Trim();
        if (string.IsNullOrWhiteSpace(s) || s.Length > max) throw AppException.Invalid($"Trường bắt buộc tối đa {max} ký tự.");
        return s;
    }
    public static void ValidatePassword(string? password)
    {
        if (password == null || password.Length is < 12 or > 128 || !password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            throw AppException.Invalid("Mật khẩu cần 12–128 ký tự, có chữ và số.");
    }
    public static string OptionalNote(string? value)
    {
        value = value?.Trim() ?? "";
        if (value.Length > 2000) throw AppException.Invalid("Ghi chú tối đa 2000 ký tự.");
        return value;
    }
    public static void Check(IdentityResult result)
    {
        if (!result.Succeeded) throw AppException.Invalid(string.Join(" ", result.Errors.Select(x => x.Description)));
    }
    public void Audit(string actor, string action, string target, object detail) => db.AuditEvents.Add(new()
    {
        ActorId = actor, Action = action, TargetId = target, DetailJson = JsonSerializer.Serialize(detail)
    });
}
