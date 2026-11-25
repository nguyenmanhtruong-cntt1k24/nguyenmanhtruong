using Hpuni.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public static class Bootstrap
{
    public static async Task Run(HpuniDbContext db, UserManager<ApplicationUser> users,
        RoleManager<ApplicationRole> roles, string password, CancellationToken ct = default)
    {
        AdminService.ValidatePassword(password);
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "identity", ct);
        if (await db.Users.AnyAsync(ct)) throw new InvalidOperationException("Bootstrap chỉ dùng cho database chưa có tài khoản; không đặt lại dữ liệu đang có.");
        var departments = new[]
        {
            ("administration", "QT", "Phòng Quản trị"), ("executive", "BGH", "Ban Giám hiệu"),
            ("accounting", "KTTC", "Phòng Kế hoạch – Tài chính"), ("it", "CNTT", "Khoa CNTT"), ("economics", "KT", "Khoa Kinh tế")
        };
        foreach (var (id, code, name) in departments)
            if (!await db.Departments.AnyAsync(x => x.Id == id, ct))
                db.Departments.Add(new() { Id = id, Code = code, Name = name, NormalizedName = name.ToUpperInvariant() });
        await db.SaveChangesAsync(ct);
        var seed = new (string Id, string Name, string Scope, string[] Permissions)[]
        {
            ("admin", "Quản trị viên", Scopes.All, [.. Permissions.CoreAdmin, "departments.manage"]),
            ("executive", "Ban Giám hiệu", Scopes.All, ["budget.approve", "budget.read", "reports.read", "reports.export"]),
            ("khtc", "Cán bộ thẩm định KHTC", Scopes.All, ["budget.appraise", "budget.read", "reports.read"]),
            ("accounting", "Cán bộ kế toán", Scopes.All, ["budget.write", "transactions.write", "budget.read", "reports.read", "reports.export"]),
            ("resources", "Quản lý phòng và tài sản", Scopes.All, ["resources.manage", "requests.review", "inventory.manage", "logs.review", "reports.read", "reports.export"]),
            ("lecturer", "Cán bộ / Giảng viên", Scopes.Self, ["rooms.book", "devices.borrow", "logs.write"])
        };
        foreach (var (id, name, scope, permissions) in seed)
        {
            if (await db.Roles.AnyAsync(x => x.Id == id, ct)) throw new InvalidOperationException("Vai trò bootstrap đã tồn tại; cần kiểm tra database.");
            AdminService.Check(await roles.CreateAsync(new() { Id = id, Name = name, Scope = scope, BuiltIn = true }));
            db.RolePermissions.AddRange(permissions.Select(p => new RolePermission { RoleId = id, Permission = p }));
        }
        var admin = new ApplicationUser { UserName = "admin.hpuni", Email = "admin@hpuni.test",
            DisplayName = "Quản trị hệ thống", DepartmentId = "administration", Scope = Scopes.All };
        AdminService.Check(await users.CreateAsync(admin, password));
        db.UserRoles.Add(new() { UserId = admin.Id, RoleId = "admin" });
        db.AuditEvents.Add(new() { ActorId = admin.Id, Action = "system.bootstrap", TargetId = admin.Id });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }
}
