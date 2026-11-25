using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public static class FinanceConfiguration
{
    public static void ConfigureFinance(this ModelBuilder b)
    {
        b.Entity<BudgetProposal>(e => {
            e.Property(x=>x.RowVersion).IsRowVersion();
            e.Property(x=>x.Title).HasMaxLength(150); e.Property(x=>x.Purpose).HasMaxLength(2000);
            e.Property(x=>x.Status).HasMaxLength(20); e.Property(x=>x.Category).HasMaxLength(30);
            e.Property(x=>x.ReviewNote).HasMaxLength(2000); e.Property(x=>x.Amount).HasPrecision(18,0);
            e.HasOne<Department>().WithMany().HasForeignKey(x=>x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.OwnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.ReviewerId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_Proposal_Amount","[SoTien] > 0 AND [Nam] BETWEEN 2000 AND 2100"));
        });
        b.Entity<BudgetAllocation>(e => {
            e.HasKey(x=>x.ProposalId);
            e.Property(x=>x.Amount).HasPrecision(18,0);
            e.HasOne<BudgetProposal>().WithOne().HasForeignKey<BudgetAllocation>(x=>x.ProposalId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_Allocation_Amount","[SoTien] > 0"));
        });
        b.Entity<PaymentRequest>(e => {
            e.Property(x=>x.RowVersion).IsRowVersion();e.Property(x=>x.Amount).HasPrecision(18,0);
            e.Property(x=>x.Content).HasMaxLength(2000);e.Property(x=>x.Status).HasMaxLength(20);
            e.HasIndex(x=>x.MaintenanceId).IsUnique().HasFilter("[MaPhieuBaoTri] IS NOT NULL");
            e.HasOne<MaintenanceOrder>().WithMany().HasForeignKey(x=>x.MaintenanceId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Department>().WithMany().HasForeignKey(x=>x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_Payment_Amount","[SoTien] > 0 AND [Nam] BETWEEN 2000 AND 2100"));
        });
        b.Entity<FinanceEntry>(e => {
            e.Property(x=>x.Amount).HasPrecision(18,0);e.Property(x=>x.Category).HasMaxLength(30);
            e.Property(x=>x.Kind).HasMaxLength(10);e.Property(x=>x.Voucher).HasMaxLength(100);
            e.Ignore(x=>x.NormalizedVoucher);e.Property(x=>x.Content).HasMaxLength(2000);
            e.Property(x=>x.Voucher).UseCollation("Latin1_General_100_CI_AS");
            e.HasIndex(x=>new{x.Year,x.Voucher}).IsUnique();
            e.HasIndex(x=>x.PaymentRequestId);
            e.Property(x=>x.FundingProposalId).HasColumnName("MaDuToanNhanKinhPhi");
            e.HasOne<BudgetProposal>().WithMany().HasForeignKey(x=>x.FundingProposalId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x=>new{x.DepartmentId,x.Year,x.Category});
            e.HasIndex(x=>x.Kind);
            // Supplemental-table foreign keys are installed explicitly by ExactFixSchema.

            // The core voucher's actor/amount constraints are installed by the exact-schema migration.
        });
    }
}
