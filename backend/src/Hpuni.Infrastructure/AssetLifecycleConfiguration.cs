using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public static class AssetLifecycleConfiguration
{
    public static void ConfigureAssetLifecycle(this ModelBuilder b)
    {
        b.Entity<FixedAssetProfile>(e => {
            e.HasKey(x => x.AssetGroupId); e.Property(x => x.AcquisitionValue).HasPrecision(18, 0);
            e.Property(x => x.Source).HasMaxLength(200); e.Property(x => x.Reference).HasMaxLength(150); e.Property(x => x.Note).HasMaxLength(2000);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasOne<AssetGroup>().WithOne().HasForeignKey<FixedAssetProfile>(x => x.AssetGroupId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ActorId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t => t.HasCheckConstraint("CK_FixedAssetProfile_Value", "[GiaTriMua] BETWEEN 1 AND 1000000000000 AND [NgayMua] >= '1900-01-01'"));
        });
        b.Entity<AssetDisposal>(e => {
            e.Property(x => x.Reason).HasMaxLength(2000); e.Property(x => x.Reference).HasMaxLength(150); e.Property(x => x.ReviewNote).HasMaxLength(2000);
            e.Property(x => x.Status).HasMaxLength(20); e.Property(x => x.RowVersion).IsRowVersion();
            e.HasIndex(x => new { x.DepartmentId, x.Status, x.CreatedAt }); e.HasIndex(x => new { x.AssetGroupId, x.Status });
            e.HasOne<AssetGroup>().WithMany().HasForeignKey(x => x.AssetGroupId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ActorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ReviewerId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t => {
                t.HasCheckConstraint("CK_AssetDisposal_Quantity", "[SoLuongTot] BETWEEN 0 AND 1000000 AND [SoLuongHong] BETWEEN 0 AND 1000000 AND [SoLuongTot]+[SoLuongHong] BETWEEN 1 AND 1000000");
                t.HasCheckConstraint("CK_AssetDisposal_Status", "([TrangThai] = 'pending' AND [MaNguoiDuyet] IS NULL AND [NgayDuyet] IS NULL) OR ([TrangThai] IN ('approved','rejected') AND [MaNguoiDuyet] IS NOT NULL AND [NgayDuyet] IS NOT NULL AND [MaNguoiDuyet] <> [MaNguoiThucHien])");
            });
        });
    }
}
