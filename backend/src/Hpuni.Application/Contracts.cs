using Hpuni.Domain;

namespace Hpuni.Application;

public record UserView(string Id, string UserName, string Email, string DisplayName, string DepartmentId,
    bool Active, string Scope, string Version, string[] RoleIds, string[] DepartmentIds, string[] Denied,
    RoleGrant[] Grants, string[]? OverrideRoleIds = null);
public record RoleView(string Id, string Name, string Scope, bool BuiltIn, string Version, string[] Permissions);
public record DepartmentView(string Id, string Code, string Name, string Notes, bool Active, string Version);
public record CreateUser(string UserName, string Email, string DisplayName, string DepartmentId,
    string Password, string[] RoleIds);
public record UpdateUser(string Email, string DisplayName, string DepartmentId, string[] RoleIds, string Version);
public record ChangeStatus(bool Active, string Version, string? Reason);
public record ConfigureScope(string Scope, string[] DepartmentIds, string[] Denied, string Version);
public record ConfigureOverride(string RoleId, bool Inherit, string Scope, string[] Permissions, string Version);
public record SaveRole(string Name, string Scope, string[] Permissions, string? Version);
public record SaveDepartment(string Code, string Name, string? Notes, bool Active, string? Version);
public record SaveRoom(string Code, string Name, string Type, string DepartmentId, int Capacity, bool Active, string? Version);
public record RoomView(string Id, string Code, string Name, string Type, string DepartmentId, int Capacity, bool Active, string Version);
public record SaveRoomRequest(string RoomId, DateTimeOffset Start, DateTimeOffset End, int Attendees, string? Purpose, string? Version);
public record RoomRequestView(string Id, string RoomId, string OwnerId, string DepartmentId, DateTimeOffset Start,
    DateTimeOffset End, int Attendees, string Purpose, string Status, string? ReviewedBy, string ReviewNote, string Version);
public record RoomAction(string Version, string? Note);
public record SaveRoomBlock(DateTimeOffset Start, DateTimeOffset End, string? Note);
public record RoomBlockView(string Id, string RoomId, DateTimeOffset Start, DateTimeOffset End, bool Active, string Version);
public record TimeSlot(DateTimeOffset Start, DateTimeOffset End);
public record SaveRoomLog(bool Used, DateTimeOffset? ActualStart, DateTimeOffset? ActualEnd, int? Attendees,
    string? Condition, string? Issue, string? Version);
public record ReviewRoomLog(string Result, string? Note, string Version);
public record RoomLogView(string Id, string RequestId, bool Used, DateTimeOffset? ActualStart, DateTimeOffset? ActualEnd,
    int? Attendees, string Condition, string Issue, string Status, string? ReviewedBy, string ReviewNote, string Version);

public class AppException(int status, string code, string message) : Exception(message)
{
    public int Status { get; } = status;
    public string Code { get; } = code;
    public static AppException Invalid(string message) => new(400, "VALIDATION_ERROR", message);
    public static AppException Forbidden() => new(403, "FORBIDDEN", "Bạn không có quyền thực hiện thao tác này.");
    public static AppException Missing() => new(404, "NOT_FOUND", "Không tìm thấy hồ sơ trong phạm vi được cấp.");
    public static AppException Stale() => new(409, "STALE_VERSION", "Dữ liệu đã thay đổi. Vui lòng tải lại.");
}

public interface IAccessService
{
    Task<AccessProfile> GetProfile(string userId, CancellationToken ct = default);
}
