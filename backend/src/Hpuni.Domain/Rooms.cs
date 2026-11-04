namespace Hpuni.Domain;

public static class RoomRequestStatuses
{
    public const string Draft = "draft", Pending = "pending", Approved = "approved", Rejected = "rejected", Cancelled = "cancelled";
}

public class Room
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public int Capacity { get; set; }
    public bool Active { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}

public class RoomRequest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RoomId { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public string DepartmentId { get; set; } = "";
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public int Attendees { get; set; }
    public string Purpose { get; set; } = "";
    public string Status { get; set; } = RoomRequestStatuses.Draft;
    public string? ReviewedBy { get; set; }
    public string ReviewNote { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public byte[] RowVersion { get; set; } = [];
}

public class RoomReservation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RoomId { get; set; } = "";
    public string? RequestId { get; set; }
    public string Kind { get; set; } = "booking";
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool Active { get; set; } = true;
    public string Note { get; set; } = "";
    public byte[] RowVersion { get; set; } = [];
}

public class IdempotencyRecord
{
    public string ActorId { get; set; } = "";
    public string Key { get; set; } = "";
    public string Operation { get; set; } = "";
    public string PayloadHash { get; set; } = "";
    public string ResultJson { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class RoomUseLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string RequestId { get; set; } = "";
    public bool Used { get; set; }
    public DateTimeOffset? ActualStart { get; set; }
    public DateTimeOffset? ActualEnd { get; set; }
    public int? Attendees { get; set; }
    public string Condition { get; set; } = "";
    public string Issue { get; set; } = "";
    public string Status { get; set; } = "submitted";
    public string? ReviewedBy { get; set; }
    public string ReviewNote { get; set; } = "";
    public byte[] RowVersion { get; set; } = [];
}
