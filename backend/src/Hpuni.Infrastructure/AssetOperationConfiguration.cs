using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public static class AssetOperationConfiguration
{
    public static void ConfigureAssetOperations(this ModelBuilder b)
    {
        b.Entity<MaintenanceOrder>(e => {
            e.Property(x=>x.RowVersion).IsRowVersion();
            e.Property(x=>x.Mode).HasMaxLength(20); e.Property(x=>x.Status).HasMaxLength(20);
            e.Property(x=>x.Result).HasMaxLength(20); e.Property(x=>x.Content).HasMaxLength(2000); e.Property(x=>x.Note).HasMaxLength(2000);
            e.Property(x=>x.Cost).HasPrecision(18,0);
            e.HasOne<AssetGroup>().WithMany().HasForeignKey(x=>x.AssetGroupId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Department>().WithMany().HasForeignKey(x=>x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x=>new{x.AssetGroupId,x.Status});
            e.ToTable(t=>t.HasCheckConstraint("CK_Maintenance_Valid", "[SoLuong] BETWEEN 1 AND 1000000 AND [BatDau] < [KetThuc] AND [CheDo] IN ('service','repair') AND [TrangThai] IN ('planned','running','completed','cancelled') AND [ChiPhi] BETWEEN 0 AND 1000000000000 AND (([TrangThai]='completed' AND [KetQua] IN ('good','unavailable')) OR ([TrangThai]<>'completed' AND [KetQua]=''))"));
        });
        b.Entity<InventorySession>(e=>{
            e.Property(x=>x.RowVersion).IsRowVersion(); e.Property(x=>x.Name).HasMaxLength(150); e.Property(x=>x.Status).HasMaxLength(20);
            e.HasOne<Department>().WithMany().HasForeignKey(x=>x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_Inventory_Status","[TrangThai] IN ('open','completed')"));
        });
        b.Entity<InventoryItem>(e=>{
            e.HasKey(x=>new{x.SessionId,x.AssetGroupId});
            e.HasOne<InventorySession>().WithMany().HasForeignKey(x=>x.SessionId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<AssetGroup>().WithMany().HasForeignKey(x=>x.AssetGroupId).OnDelete(DeleteBehavior.Restrict);
            e.Property(x=>x.Code).HasMaxLength(30); e.Property(x=>x.Name).HasMaxLength(150); e.Property(x=>x.AssetVersion).HasMaxLength(40);
            e.Property(x=>x.Condition).HasMaxLength(20); e.Property(x=>x.Note).HasMaxLength(2000);
            e.ToTable(t=>t.HasCheckConstraint("CK_Inventory_Count","[SoLuongDuKien] BETWEEN 0 AND 1000000 AND [SoLuongHongDuKien]>=0 AND [SoLuongMatDuKien]>=0 AND [SoLuongHongDuKien]+[SoLuongMatDuKien]<=[SoLuongDuKien] AND ([SoLuongThucTe] IS NULL OR [SoLuongThucTe] BETWEEN 0 AND 1000000) AND [TinhTrang] IN ('','good','damaged','missing')"));
        });
        b.Entity<EquipmentUseLog>(e=>{
            e.Property(x=>x.RowVersion).IsRowVersion(); e.HasIndex(x=>x.LoanId).IsUnique();
            e.HasOne<EquipmentLoan>().WithMany().HasForeignKey(x=>x.LoanId).OnDelete(DeleteBehavior.Restrict);
            e.Property(x=>x.Condition).HasMaxLength(2000); e.Property(x=>x.Issue).HasMaxLength(2000); e.Property(x=>x.ReviewNote).HasMaxLength(2000); e.Property(x=>x.Status).HasMaxLength(20);
            e.ToTable(t=>t.HasCheckConstraint("CK_EquipmentLog_Valid","[TrangThai] IN ('submitted','needs_changes','confirmed') AND (([DaSuDung]=0 AND [BatDauThucTe] IS NULL AND [KetThucThucTe] IS NULL AND [SoLuong] IS NULL) OR ([DaSuDung]=1 AND [BatDauThucTe] IS NOT NULL AND [KetThucThucTe] IS NOT NULL AND [SoLuong] IS NOT NULL AND [BatDauThucTe]<[KetThucThucTe] AND [SoLuong] BETWEEN 1 AND 1000000))"));
        });
    }
}
