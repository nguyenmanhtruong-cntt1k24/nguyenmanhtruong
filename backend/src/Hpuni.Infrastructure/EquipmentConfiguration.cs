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
        e.ToTable(t => t.HasCheckConstraint("CK_Asset_Quantity", "[TongSo] BETWEEN 0 AND 1000000 AND [SoLuongHong] BETWEEN 0 AND 1000000 AND [SoLuongMat] BETWEEN 0 AND 1000000 AND [SoLuongHong] + [SoLuongMat] <= [TongSo]"));
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
            t.HasCheckConstraint("CK_Loan_Quantity", "[SoLuong] BETWEEN 1 AND 1000000 AND [SoLuongDaTra] >= 0 AND [SoLuongDaTra] <= [SoLuong] AND ([SoLuongBaoDaNhan] IS NULL OR [SoLuongBaoDaNhan] BETWEEN 0 AND [SoLuong])");
            t.HasCheckConstraint("CK_Loan_Interval", "[BatDau] < [KetThuc]");
            t.HasCheckConstraint("CK_Loan_Status", "[TrangThai] IN ('draft','pending','approved','rejected','cancelled','ready','receipt_disputed','borrowed','return_pending','completed') AND (([TrangThai] IN ('ready','receipt_disputed','borrowed','return_pending','completed') AND [NgayBanGiao] IS NOT NULL) OR ([TrangThai] IN ('draft','pending','approved','rejected','cancelled') AND [NgayBanGiao] IS NULL))");
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
        e.HasIndex(x => x.LoanId).IsUnique().HasFilter("[NgayNghiemThu] IS NULL");
        e.ToTable(t => t.HasCheckConstraint("CK_Return_Quantity", "[SoLuongTot] BETWEEN 0 AND 1000000 AND [SoLuongHong] BETWEEN 0 AND 1000000 AND [SoLuongMat] BETWEEN 0 AND 1000000 AND [SoLuongTot]+[SoLuongHong]+[SoLuongMat] BETWEEN 1 AND 1000000 AND (([NgayNghiemThu] IS NULL AND [SoLuongTotNghiemThu] IS NULL AND [SoLuongHongNghiemThu] IS NULL AND [SoLuongMatNghiemThu] IS NULL AND [MaNguoiNghiemThu] IS NULL) OR ([NgayNghiemThu] IS NOT NULL AND [MaNguoiNghiemThu] IS NOT NULL AND [SoLuongTotNghiemThu] IS NOT NULL AND [SoLuongHongNghiemThu] IS NOT NULL AND [SoLuongMatNghiemThu] IS NOT NULL AND [SoLuongTotNghiemThu] BETWEEN 0 AND 1000000 AND [SoLuongHongNghiemThu] BETWEEN 0 AND 1000000 AND [SoLuongMatNghiemThu] BETWEEN 0 AND 1000000 AND [SoLuongTotNghiemThu]+[SoLuongHongNghiemThu]+[SoLuongMatNghiemThu] BETWEEN 1 AND [SoLuongTot]+[SoLuongHong]+[SoLuongMat]))"));
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
        e.ToTable(t => t.HasCheckConstraint("CK_Movement_Quantity", "([PhanLoai] IN ('disposal.good','disposal.damaged') AND [SoLuong] BETWEEN -1000000 AND -1) OR ([PhanLoai] NOT IN ('disposal.good','disposal.damaged') AND [SoLuong] BETWEEN 1 AND 1000000)"));
    }
}
