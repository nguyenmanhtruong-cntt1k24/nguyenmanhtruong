using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hpuni.Infrastructure;

public class AssetGroupConfiguration : IEntityTypeConfiguration<AssetGroup>
{
    public void Configure(EntityTypeBuilder<AssetGroup> e)
    {
        e.Property(x => x.Code).HasMaxLength(30);
        e.Property(x => x.Name).HasMaxLength(150);
        e.Property(x => x.Category).HasMaxLength(100);
        e.Property(x => x.Unit).HasMaxLength(30);
        e.Property(x => x.RowVersion).IsRowVersion();
        e.HasIndex(x => x.Code).IsUnique();
        e.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        e.ToTable(t => t.HasCheckConstraint("CK_Asset_Quantity", "[Total] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Lost] BETWEEN 0 AND 1000000 AND [Damaged] + [Lost] <= [Total]"));
    }
}
public class EquipmentLoanConfiguration : IEntityTypeConfiguration<EquipmentLoan>
{
    public void Configure(EntityTypeBuilder<EquipmentLoan> e)
    {
        e.Property(x => x.Status).HasMaxLength(30);
        e.Property(x => x.Purpose).HasMaxLength(2000);
        e.Property(x => x.Note).HasMaxLength(2000);
        e.Property(x => x.RowVersion).IsRowVersion();
        e.HasOne(x => x.Asset).WithMany().HasForeignKey(x => x.AssetGroupId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        e.HasIndex(x => new { x.AssetGroupId, x.Status, x.Start, x.End });
        e.HasIndex(x => new { x.OwnerId, x.CreatedAt });
        e.ToTable(t => {
            t.HasCheckConstraint("CK_Loan_Quantity", "[Quantity] BETWEEN 1 AND 1000000 AND [Returned] >= 0 AND [Returned] <= [Quantity] AND ([ReportedReceived] IS NULL OR [ReportedReceived] BETWEEN 0 AND [Quantity])");
            t.HasCheckConstraint("CK_Loan_Interval", "[Start] < [End]");
            t.HasCheckConstraint("CK_Loan_Status", "[Status] IN ('draft','pending','approved','rejected','cancelled','ready','receipt_disputed','borrowed','return_pending','completed') AND (([Status] IN ('ready','receipt_disputed','borrowed','return_pending','completed') AND [HandedAt] IS NOT NULL) OR ([Status] IN ('draft','pending','approved','rejected','cancelled') AND [HandedAt] IS NULL))");
        });
    }
}
public class EquipmentReturnConfiguration : IEntityTypeConfiguration<EquipmentReturn>
{
    public void Configure(EntityTypeBuilder<EquipmentReturn> e)
    {
        e.Property(x => x.Note).HasMaxLength(2000);
        e.Property(x => x.AcceptanceNote).HasMaxLength(2000);
        e.HasOne<EquipmentLoan>().WithMany().HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.AcceptedBy).OnDelete(DeleteBehavior.Restrict);
        e.HasIndex(x => x.LoanId).IsUnique().HasFilter("[AcceptedAt] IS NULL");
        e.ToTable(t => t.HasCheckConstraint("CK_Return_Quantity", "[Good] BETWEEN 0 AND 1000000 AND [Damaged] BETWEEN 0 AND 1000000 AND [Lost] BETWEEN 0 AND 1000000 AND [Good]+[Damaged]+[Lost] BETWEEN 1 AND 1000000 AND (([AcceptedAt] IS NULL AND [AcceptedGood] IS NULL AND [AcceptedDamaged] IS NULL AND [AcceptedLost] IS NULL AND [AcceptedBy] IS NULL) OR ([AcceptedAt] IS NOT NULL AND [AcceptedBy] IS NOT NULL AND [AcceptedGood] IS NOT NULL AND [AcceptedDamaged] IS NOT NULL AND [AcceptedLost] IS NOT NULL AND [AcceptedGood] BETWEEN 0 AND 1000000 AND [AcceptedDamaged] BETWEEN 0 AND 1000000 AND [AcceptedLost] BETWEEN 0 AND 1000000 AND [AcceptedGood]+[AcceptedDamaged]+[AcceptedLost] BETWEEN 1 AND [Good]+[Damaged]+[Lost]))"));
    }
}
public class EquipmentMovementConfiguration : IEntityTypeConfiguration<EquipmentMovement>
{
    public void Configure(EntityTypeBuilder<EquipmentMovement> e)
    {
        e.Property(x => x.Kind).HasMaxLength(30);
        e.Property(x => x.Note).HasMaxLength(2000);
        e.HasOne<AssetGroup>().WithMany().HasForeignKey(x => x.AssetGroupId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<EquipmentLoan>().WithMany().HasForeignKey(x => x.LoanId).OnDelete(DeleteBehavior.Restrict);
        e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ActorId).OnDelete(DeleteBehavior.Restrict);
        e.HasIndex(x => new { x.AssetGroupId, x.At });
        e.ToTable(t => t.HasCheckConstraint("CK_Movement_Quantity", "([Kind] IN ('disposal.good','disposal.damaged') AND [Quantity] BETWEEN -1000000 AND -1) OR ([Kind] NOT IN ('disposal.good','disposal.damaged') AND [Quantity] BETWEEN 1 AND 1000000)"));
    }
}
