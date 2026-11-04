using System.Data;
using Hpuni.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hpuni.Infrastructure;

public static class TransactionLocks
{
    public static async Task Acquire(HpuniDbContext db, string resource, CancellationToken ct = default)
    {
        // SQL identifiers commonly use a case-insensitive collation while applock names are binary.
        // GUID source IDs must map to one lock even when the caller changes their letter casing.
        var separator=resource.LastIndexOf(':');
        if(separator>=0&&Guid.TryParse(resource[(separator+1)..],out var id))
            resource=resource[..(separator+1)]+id.ToString("D");
        if (db.Database.CurrentTransaction == null) throw new InvalidOperationException("Transaction required.");
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.Transaction = db.Database.CurrentTransaction.GetDbTransaction();
        command.CommandText = "DECLARE @result int; EXEC @result = sys.sp_getapplock @Resource=@resource, @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=10000; SELECT @result;";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@resource";
        parameter.DbType = DbType.String;
        parameter.Value = "hpuni:" + resource;
        command.Parameters.Add(parameter);
        var result = Convert.ToInt32(await command.ExecuteScalarAsync(ct));
        if (result < 0) throw new AppException(409, "RESOURCE_BUSY", "Dữ liệu đang được xử lý. Vui lòng thử lại.");
    }
}
