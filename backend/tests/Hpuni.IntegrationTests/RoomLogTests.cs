using System.Net;
using System.Net.Http.Json;
using Hpuni.Application;
using Hpuni.Domain;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class RoomLogTests(SqlFactory f) : IClassFixture<SqlFactory>
{
    [Fact]
    public async Task NoUseReviewResubmissionAndConfirmationKeepOneLogAndHistory()
    {
        using var admin = await f.Login();
        var lecturer = await SqlFactory.Create(admin);
        var manager = await SqlFactory.Create(admin, "resources", "administration");
        Assert.Equal(HttpStatusCode.OK, (await admin.PutAsJsonAsync($"/api/v1/users/{manager.Id}/scope", new ConfigureScope("all", [], [], manager.Version))).StatusCode);
        var requestId = Guid.NewGuid().ToString();
        // Historical approved record fixture: advances no production clock and touches only this test database.
        await using (var scope = f.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
            var room = new Room { Code = "HISTORY", Name = "Phòng lịch sử", Type = "Phòng học", DepartmentId = "administration", Capacity = 50 };
            db.Rooms.Add(room);
            db.RoomRequests.Add(new() { Id = requestId, RoomId = room.Id, OwnerId = lecturer.Id, DepartmentId = "it",
                Status = "approved", Start = DateTimeOffset.UtcNow.AddDays(-1).AddHours(-2), End = DateTimeOffset.UtcNow.AddDays(-1), Attendees = 25 });
            await db.SaveChangesAsync();
        }
        using var l = await f.Login(lecturer.UserName); using var m = await f.Login(manager.UserName);
        var first = await l.PutAsJsonAsync($"/api/v1/room-requests/{requestId}/log", new SaveRoomLog(false, null, null, null, "", null, null));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        var log = (await first.Content.ReadFromJsonAsync<RoomLogView>())!;
        Assert.False(log.Used);
        var review = await m.PostAsJsonAsync($"/api/v1/room-requests/{requestId}/log/review", new ReviewRoomLog("needs_changes", "Bổ sung kết quả", log.Version));
        Assert.Equal(HttpStatusCode.OK, review.StatusCode);
        log = (await review.Content.ReadFromJsonAsync<RoomLogView>())!;
        var resubmit = await l.PutAsJsonAsync($"/api/v1/room-requests/{requestId}/log", new SaveRoomLog(true,
            DateTimeOffset.UtcNow.AddDays(-1).AddHours(-2), DateTimeOffset.UtcNow.AddDays(-1), 20, "", "", log.Version));
        Assert.Equal(HttpStatusCode.OK, resubmit.StatusCode);
        var amended = (await resubmit.Content.ReadFromJsonAsync<RoomLogView>())!;
        Assert.Equal(log.Id, amended.Id);
        Assert.Equal(HttpStatusCode.OK, (await m.PostAsJsonAsync($"/api/v1/room-requests/{requestId}/log/review",
            new ReviewRoomLog("confirmed", null, amended.Version))).StatusCode);
        var confirmed = (await m.GetFromJsonAsync<RoomLogView>($"/api/v1/room-requests/{requestId}/log?review=true"))!;
        Assert.Equal("confirmed", confirmed.Status);
        Assert.Equal(HttpStatusCode.BadRequest, (await l.PutAsJsonAsync($"/api/v1/room-requests/{requestId}/log",
            new SaveRoomLog(false, null, null, null, "", "", confirmed.Version))).StatusCode);
        await using var checkScope = f.Services.CreateAsyncScope();
        var checkDb = checkScope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        Assert.Equal(1, await checkDb.RoomUseLogs.CountAsync(x => x.RequestId == requestId));
        Assert.Equal(4, await checkDb.AuditEvents.CountAsync(x => x.TargetId == confirmed.Id));
    }
}
