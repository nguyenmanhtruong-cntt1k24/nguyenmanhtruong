using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

public class IdempotentCommands(HpuniDbContext db)
{
    public async Task<T> Run<T>(string actor, string key, string operation, object payload,
        Func<Task> authorize, Func<Task<T>> action, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length > 100 || key.Any(char.IsControl))
            throw AppException.Invalid("Cần Idempotency-Key từ 1 đến 100 ký tự.");
        static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)));
        var hash = Hash(JsonSerializer.Serialize(payload));
        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await TransactionLocks.Acquire(db, "idempotency:" + Hash(actor + ":" + key), ct);
        await authorize(); // Replays still require current access.
        var existing = await db.IdempotencyRecords.SingleOrDefaultAsync(x => x.ActorId == actor && x.Key == key, ct);
        if (existing != null)
        {
            if (existing.Operation != operation || existing.PayloadHash != hash)
                throw new AppException(409, "IDEMPOTENCY_CONFLICT", "Khóa gửi lặp đã dùng với nội dung khác.");
            await tx.CommitAsync(ct);
            return JsonSerializer.Deserialize<T>(existing.ResultJson)!;
        }
        var result = await action();
        db.IdempotencyRecords.Add(new() { ActorId = actor, Key = key, Operation = operation,
            PayloadHash = hash, ResultJson = JsonSerializer.Serialize(result) });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return result;
    }
}
