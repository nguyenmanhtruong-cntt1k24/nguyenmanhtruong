using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace Hpuni.Infrastructure;

// EF's documented extension point for physical history column names.
#pragma warning disable EF1001
public sealed class VietnameseHistoryRepository(HistoryRepositoryDependencies dependencies)
    : SqlServerHistoryRepository(dependencies)
{
    protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
    {
        base.ConfigureTable(history);
        history.Property(x => x.MigrationId).HasColumnName("MaCapNhat");
        history.Property(x => x.ProductVersion).HasColumnName("PhienBanSanPham");
    }
}
#pragma warning restore EF1001
