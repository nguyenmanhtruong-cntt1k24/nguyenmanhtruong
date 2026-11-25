using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public static class ExactFixConfiguration
{
    public static void ConfigureExactFix(this ModelBuilder b)
    {
        b.Entity<ApplicationRole>().ToTable(t => t.UseSqlOutputClause(false));
        b.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable(t => t.UseSqlOutputClause(false));
        b.Entity<RolePermission>().ToTable(t => t.UseSqlOutputClause(false));
        b.Entity<RoleAccountOverride>().ToTable(t => t.UseSqlOutputClause(false));
        b.Entity<ApplicationUser>().SplitToTable("BaoMatNguoiDung", "QuanTriVien", t =>
        {
            t.Property(x => x.Id);
            t.Property(x => x.Scope);
            t.Property(x => x.Active);
            t.Property(x => x.RowVersion);
            t.Property(x => x.SecurityStamp);
            t.Property(x => x.LockoutEnd);
            t.Property(x => x.LockoutEnabled);
            t.Property(x => x.AccessFailedCount);
        });
        b.Entity<LoginSession>(e =>
        {
            e.HasKey(x => new { x.UserId, x.CreatedAt });
            e.HasIndex(x => x.Id).IsUnique();
            e.SplitToTable("DinhDanhPhienDangNhap", "QuanTriVien", t =>
            {
                t.Property(x => x.UserId);
                t.Property(x => x.CreatedAt);
                t.Property(x => x.Id);
                t.Property(x => x.RevokedAt);
            });
        });
        b.Entity<AuditEvent>().SplitToTable("DoiTuongNhatKy", "QuanTriVien", t =>
        {
            t.Property(x => x.Id);
            t.Property(x => x.TargetId);
        });
        var finance = b.Entity<FinanceEntry>();
        finance.Metadata.RemoveCheckConstraint("CK_Entry_Amount");
        finance.ToTable("ChiTietChungTu", "KeToan");
        finance.Property(x => x.ActorId).HasMaxLength(450);
        finance.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        finance.HasOne<PaymentRequest>().WithMany().HasForeignKey(x => x.PaymentRequestId).OnDelete(DeleteBehavior.Restrict);
        finance.HasIndex(x => new { x.DepartmentId, x.Year, x.Date, x.Id });
        finance.SplitToTable("ChungTuTaiChinh", "KeToan", t =>
        {
            t.Property(x => x.Id);
            t.Property(x => x.ActorId);
            t.Property(x => x.CreatedAt);
            t.Property(x => x.Amount);
            t.Property(x => x.Kind);
            t.Property(x => x.Content);
        });
        b.Entity<PermissionAssignment>(e =>
        {
            e.ToTable("PhanQuyen", "QuanTriVien");
            e.HasKey(x => new { x.UserId, x.RoleId, x.Permission });
            e.Property(x => x.RoleId).HasColumnName("MaVaiTro").HasMaxLength(450);
            e.Property(x => x.RoleName).HasColumnName("TenVaiTro").HasMaxLength(256).UseCollation("Latin1_General_100_CI_AS");
            e.Property(x => x.UserId).HasColumnName("MaNguoiDung").HasMaxLength(450);
            e.Property(x => x.Permission).HasColumnName("Quyen").HasMaxLength(100);
        });
    }
}
