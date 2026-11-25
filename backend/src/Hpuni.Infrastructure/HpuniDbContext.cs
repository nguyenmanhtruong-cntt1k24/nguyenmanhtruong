using Hpuni.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class HpuniDbContext(DbContextOptions<HpuniDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public DbSet<PermissionAssignment> PermissionAssignments => Set<PermissionAssignment>();
    public DbSet<FiscalPeriod> FiscalPeriods => Set<FiscalPeriod>();
    public DbSet<FixedAssetProfile> FixedAssetProfiles => Set<FixedAssetProfile>();
    public DbSet<AssetDisposal> AssetDisposals => Set<AssetDisposal>();
    public DbSet<BudgetAdjustment> BudgetAdjustments => Set<BudgetAdjustment>();
    public DbSet<FinanceReversal> FinanceReversals => Set<FinanceReversal>();
    public DbSet<BudgetProposal> BudgetProposals => Set<BudgetProposal>();
    public DbSet<BudgetAllocation> BudgetAllocations => Set<BudgetAllocation>();
    public DbSet<PaymentRequest> PaymentRequests => Set<PaymentRequest>();
    public DbSet<FinanceEntry> FinanceEntries => Set<FinanceEntry>();
    public DbSet<VoucherAttachment> VoucherAttachments => Set<VoucherAttachment>();
    public DbSet<VoucherAttachmentData> VoucherAttachmentData => Set<VoucherAttachmentData>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RoleAccountOverride> RoleAccountOverrides => Set<RoleAccountOverride>();
    public DbSet<UserDepartmentScope> UserDepartmentScopes => Set<UserDepartmentScope>();
    public DbSet<UserPermissionDenial> UserPermissionDenials => Set<UserPermissionDenial>();
    public DbSet<LoginSession> LoginSessions => Set<LoginSession>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomRequest> RoomRequests => Set<RoomRequest>();
    public DbSet<RoomReservation> RoomReservations => Set<RoomReservation>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public DbSet<RoomUseLog> RoomUseLogs => Set<RoomUseLog>();
    public DbSet<AssetGroup> AssetGroups => Set<AssetGroup>();
    public DbSet<EquipmentLoan> EquipmentLoans => Set<EquipmentLoan>();
    public DbSet<EquipmentReturn> EquipmentReturns => Set<EquipmentReturn>();
    public DbSet<EquipmentMovement> EquipmentMovements => Set<EquipmentMovement>();
    public DbSet<MaintenanceOrder> MaintenanceOrders => Set<MaintenanceOrder>();
    public DbSet<InventorySession> InventorySessions => Set<InventorySession>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<EquipmentUseLog> EquipmentUseLogs => Set<EquipmentUseLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ReplaceService<Microsoft.EntityFrameworkCore.Migrations.IHistoryRepository, VietnameseHistoryRepository>();
    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        // These optional Identity features have no route, provider or UI in this application.
        b.Ignore<IdentityUserClaim<string>>();
        b.Ignore<IdentityRoleClaim<string>>();
        b.Ignore<IdentityUserLogin<string>>();
        b.Ignore<IdentityUserToken<string>>();
        b.ConfigureFinance();
        b.ConfigureFinanceControls();
        b.ConfigureNotifications();
        b.ConfigureAssetLifecycle();
        b.Entity<VoucherAttachment>(e=>{
            e.Property(x=>x.FileName).HasMaxLength(150);e.Property(x=>x.ContentType).HasMaxLength(50);e.Property(x=>x.Sha256).HasMaxLength(64);
            e.HasIndex(x=>x.EntryId);
            e.HasOne<FinanceEntry>().WithMany().HasForeignKey(x=>x.EntryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.ActorId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_Attachment_Size","[KichThuoc] BETWEEN 1 AND 2097152"));
        });
        b.Entity<VoucherAttachmentData>(e=>{
            e.HasKey(x=>x.AttachmentId);
            e.HasOne<VoucherAttachment>().WithOne().HasForeignKey<VoucherAttachmentData>(x=>x.AttachmentId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t=>t.HasCheckConstraint("CK_AttachmentData_Size","DATALENGTH([NoiDung]) BETWEEN 1 AND 2097152"));
        });
        b.ConfigureAssetOperations();
        b.ApplyConfiguration(new AssetGroupConfiguration());
        b.ApplyConfiguration(new EquipmentLoanConfiguration());
        b.ApplyConfiguration(new EquipmentReturnConfiguration());
        b.ApplyConfiguration(new EquipmentMovementConfiguration());
        b.Entity<ApplicationUser>(e =>
        {
            // DonVi reads the department table; SQL Server forbids it in OUTPUT.
            e.ToTable(t => t.UseSqlOutputClause(false));
            e.Property(x => x.DisplayName).HasMaxLength(150);
            e.Property(x => x.DepartmentName).HasMaxLength(150)
                .HasComputedColumnSql("[QuanTriVien].[TenDonViNguoiDung]([MaDonVi])", stored: false);
            e.Property(x => x.Scope).HasMaxLength(10);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.Ignore(x => x.NormalizedUserName);
            e.Ignore(x => x.NormalizedEmail);
            e.Ignore(x => x.EmailConfirmed);
            e.Property(x => x.PhoneNumber).HasMaxLength(25);
            e.Ignore(x => x.PhoneNumberConfirmed);
            e.Ignore(x => x.TwoFactorEnabled);
            e.Ignore(x => x.ConcurrencyStamp); // RowVersion already protects updates.
            e.Property(x => x.UserName).UseCollation("Latin1_General_100_CI_AS");
            e.Property(x => x.Email).UseCollation("Latin1_General_100_CI_AS");
            e.HasIndex(x => x.UserName).IsUnique().HasFilter("[TenDangNhap] IS NOT NULL");
            e.HasIndex(x => x.Email).IsUnique().HasFilter("[ThuDienTu] IS NOT NULL");
        });
        b.Entity<ApplicationRole>(e =>
        {
            e.Ignore(x => x.NormalizedName);
            e.Ignore(x => x.ConcurrencyStamp);
            e.Property(x => x.Name).UseCollation("Latin1_General_100_CI_AS");
            e.HasIndex(x => x.Name).IsUnique().HasFilter("[Ten] IS NOT NULL");
            e.Property(x => x.Scope).HasMaxLength(10);
            e.Property(x => x.RowVersion).IsRowVersion();
        });
        b.Entity<Department>(e =>
        {
            e.Property(x => x.Id).HasMaxLength(450);
            e.Property(x => x.Code).HasMaxLength(30);
            e.Property(x => x.Name).HasMaxLength(150);
            e.Ignore(x => x.NormalizedName);
            e.Property(x => x.Name).UseCollation("Latin1_General_100_CI_AS");
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasIndex(x => x.Code).IsUnique();
            e.HasIndex(x => x.Name).IsUnique();
        });
        b.Entity<RolePermission>(e =>
        {
            e.HasKey(x => new { x.RoleId, x.Permission });
            e.Property(x => x.Permission).HasMaxLength(100);
            e.HasOne<ApplicationRole>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<RoleAccountOverride>(e =>
        {
            e.HasKey(x => new { x.UserId, x.RoleId });
            e.Property(x => x.Scope).HasMaxLength(10);
            e.HasOne<IdentityUserRole<string>>().WithMany().HasForeignKey(x => new { x.UserId, x.RoleId }).OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<UserDepartmentScope>(e =>
        {
            e.HasKey(x => new { x.UserId, x.DepartmentId });
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<UserPermissionDenial>(e =>
        {
            e.HasKey(x => new { x.UserId, x.Permission });
            e.Property(x => x.Permission).HasMaxLength(100);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<LoginSession>(e =>
        {
            e.Property(x => x.Id).HasMaxLength(100);
            e.HasIndex(x => x.UserId);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<AuditEvent>(e =>
        {
            e.Property(x => x.ActorId).HasMaxLength(450);
            e.Property(x => x.Action).HasMaxLength(100);
            e.Property(x => x.TargetId).HasMaxLength(450);
            e.HasIndex(x => x.At);
        });
        b.Entity<Room>(e =>
        {
            e.Property(x => x.Code).HasMaxLength(30);
            e.Property(x => x.Name).HasMaxLength(150);
            e.Property(x => x.Type).HasMaxLength(100);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasIndex(x => x.Code).IsUnique();
            e.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t => t.HasCheckConstraint("CK_Room_Capacity", "[SucChua] > 0"));
        });
        b.Entity<RoomRequest>(e =>
        {
            e.Property(x => x.Status).HasMaxLength(20);
            e.Property(x => x.Purpose).HasMaxLength(2000);
            e.Property(x => x.ReviewNote).HasMaxLength(2000);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasOne<Room>().WithMany().HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.OwnerId, x.CreatedAt });
            e.ToTable(t => { t.HasCheckConstraint("CK_Request_Interval", "[BatDau] < [KetThuc]"); t.HasCheckConstraint("CK_Request_Attendees", "[SoNguoiThamDu] > 0"); });
        });
        b.Entity<RoomReservation>(e =>
        {
            e.Property(x => x.Kind).HasMaxLength(20);
            e.Property(x => x.Note).HasMaxLength(2000);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasOne<Room>().WithMany().HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<RoomRequest>().WithMany().HasForeignKey(x => x.RequestId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.RequestId).IsUnique().HasFilter("[MaPhieuDangKy] IS NOT NULL");
            e.HasIndex(x => new { x.RoomId, x.Active, x.Start, x.End });
            e.ToTable(t => t.HasCheckConstraint("CK_Reservation_Interval", "[BatDau] < [KetThuc]"));
        });
        b.Entity<IdempotencyRecord>(e =>
        {
            e.HasKey(x => new { x.ActorId, x.Key });
            e.Property(x => x.ActorId).HasMaxLength(450);
            e.Property(x => x.Key).HasMaxLength(100);
            e.Property(x => x.Operation).HasMaxLength(100);
            e.Property(x => x.PayloadHash).HasMaxLength(64);
        });
        b.Entity<RoomUseLog>(e =>
        {
            e.Property(x => x.Status).HasMaxLength(20);
            e.Property(x => x.Condition).HasMaxLength(2000);
            e.Property(x => x.Issue).HasMaxLength(2000);
            e.Property(x => x.ReviewNote).HasMaxLength(2000);
            e.Property(x => x.RowVersion).IsRowVersion();
            e.HasIndex(x => x.RequestId).IsUnique();
            e.HasOne<RoomRequest>().WithMany().HasForeignKey(x => x.RequestId).OnDelete(DeleteBehavior.Restrict);
            e.ToTable(t => t.HasCheckConstraint("CK_RoomLog_Actual", "([DaSuDung] = 0 AND [BatDauThucTe] IS NULL AND [KetThucThucTe] IS NULL AND [SoNguoiThamDu] IS NULL) OR ([DaSuDung] = 1 AND [BatDauThucTe] IS NOT NULL AND [KetThucThucTe] IS NOT NULL AND [SoNguoiThamDu] IS NOT NULL AND [BatDauThucTe] < [KetThucThucTe] AND [SoNguoiThamDu] >= 0)"));
        });
        VietnameseTableNames.Apply(b);
        ActorTableSchemas.Apply(b);
        VietnameseColumnNames.Apply(b);
        b.ConfigureExactFix();
    }
}
