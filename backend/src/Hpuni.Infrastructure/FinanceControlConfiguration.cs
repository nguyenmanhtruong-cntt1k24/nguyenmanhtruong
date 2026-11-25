using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public static class FinanceControlConfiguration
{
    public static void ConfigureFinanceControls(this ModelBuilder b)
    {
        b.Entity<FiscalPeriod>(e=>{
            e.HasKey(x=>new{x.DepartmentId,x.Year});
            e.Property(x=>x.Reason).HasMaxLength(2000);e.Property(x=>x.RowVersion).IsRowVersion();
            e.HasOne<Department>().WithMany().HasForeignKey(x=>x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.ActorId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_FiscalPeriod_Year","[Nam] BETWEEN 2000 AND 2100"));
        });
        b.Entity<BudgetAdjustment>(e=>{
            e.Property(x=>x.RowVersion).IsRowVersion();e.Property(x=>x.Amount).HasPrecision(18,0);
            e.Property(x=>x.Reason).HasMaxLength(2000);e.Property(x=>x.ReviewNote).HasMaxLength(2000);
            e.Property(x=>x.Category).HasMaxLength(30);e.Property(x=>x.Status).HasMaxLength(20);
            e.HasIndex(x=>new{x.DepartmentId,x.Year,x.Category,x.Status});
            e.HasOne<Department>().WithMany().HasForeignKey(x=>x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.ActorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.ReviewerId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_Adjustment_Amount","[SoTien] <> 0 AND ABS([SoTien]) <= 1000000000000 AND [Nam] BETWEEN 2000 AND 2100"));
        });
        b.Entity<FinanceReversal>(e=>{
            e.Property(x=>x.Reason).HasMaxLength(2000);e.HasIndex(x=>x.EntryId).IsUnique();
            e.HasOne<FinanceEntry>().WithMany().HasForeignKey(x=>x.EntryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.ActorId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
