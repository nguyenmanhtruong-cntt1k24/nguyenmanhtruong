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
            e.ToTable(t=>t.HasCheckConstraint("CK_Maintenance_Valid", "[Quantity] BETWEEN 1 AND 1000000 AND [Start] < [End] AND [Mode] IN ('service','repair') AND [Status] IN ('planned','running','completed','cancelled') AND [Cost] BETWEEN 0 AND 1000000000000 AND (([Status]='completed' AND [Result] IN ('good','unavailable')) OR ([Status]<>'completed' AND [Result]=''))"));
        });
        b.Entity<InventorySession>(e=>{
            e.Property(x=>x.RowVersion).IsRowVersion(); e.Property(x=>x.Name).HasMaxLength(150); e.Property(x=>x.Status).HasMaxLength(20);
            e.HasOne<Department>().WithMany().HasForeignKey(x=>x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_Inventory_Status","[Status] IN ('open','completed')"));
        });
        b.Entity<InventoryItem>(e=>{
            e.HasKey(x=>new{x.SessionId,x.AssetGroupId});
            e.HasOne<InventorySession>().WithMany().HasForeignKey(x=>x.SessionId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<AssetGroup>().WithMany().HasForeignKey(x=>x.AssetGroupId).OnDelete(DeleteBehavior.Restrict);
            e.Property(x=>x.Code).HasMaxLength(30); e.Property(x=>x.Name).HasMaxLength(150); e.Property(x=>x.AssetVersion).HasMaxLength(40);
            e.Property(x=>x.Condition).HasMaxLength(20); e.Property(x=>x.Note).HasMaxLength(2000);
            e.ToTable(t=>t.HasCheckConstraint("CK_Inventory_Count","[Expected] BETWEEN 0 AND 1000000 AND [ExpectedDamaged]>=0 AND [ExpectedLost]>=0 AND [ExpectedDamaged]+[ExpectedLost]<=[Expected] AND ([Actual] IS NULL OR [Actual] BETWEEN 0 AND 1000000) AND [Condition] IN ('','good','damaged','missing')"));
        });
        b.Entity<EquipmentUseLog>(e=>{
            e.Property(x=>x.RowVersion).IsRowVersion(); e.HasIndex(x=>x.LoanId).IsUnique();
            e.HasOne<EquipmentLoan>().WithMany().HasForeignKey(x=>x.LoanId).OnDelete(DeleteBehavior.Restrict);
            e.Property(x=>x.Condition).HasMaxLength(2000); e.Property(x=>x.Issue).HasMaxLength(2000); e.Property(x=>x.ReviewNote).HasMaxLength(2000); e.Property(x=>x.Status).HasMaxLength(20);
            e.ToTable(t=>t.HasCheckConstraint("CK_EquipmentLog_Valid","[Status] IN ('submitted','needs_changes','confirmed') AND (([Used]=0 AND [ActualStart] IS NULL AND [ActualEnd] IS NULL AND [Quantity] IS NULL) OR ([Used]=1 AND [ActualStart] IS NOT NULL AND [ActualEnd] IS NOT NULL AND [Quantity] IS NOT NULL AND [ActualStart]<[ActualEnd] AND [Quantity] BETWEEN 1 AND 1000000))"));
        });
    }
}
