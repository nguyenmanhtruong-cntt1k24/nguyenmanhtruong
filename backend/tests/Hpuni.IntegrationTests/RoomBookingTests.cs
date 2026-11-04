using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class RoomBookingTests(SqlFactory f) : IClassFixture<SqlFactory>
{
    private static DateTimeOffset Tomorrow(int hour) => new(DateTime.Today.AddDays(2).AddHours(hour), TimeSpan.FromHours(7));
    private static string Key() => Guid.NewGuid().ToString("N");

    private static Task<HttpResponseMessage> Send(HttpClient c, string path, object data, string? key = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = JsonContent.Create(data) };
        request.Headers.Add("Idempotency-Key", key ?? Key());
        return c.SendAsync(request);
    }

    private async Task<(HttpClient Manager, HttpClient Lecturer, RoomView Room)> Setup()
    {
        using var admin = await f.Login();
        var manager = await SqlFactory.Create(admin, "resources", "administration");
        var scope = await admin.PutAsJsonAsync($"/api/v1/users/{manager.Id}/scope", new ConfigureScope("all", [], [], manager.Version));
        Assert.Equal(HttpStatusCode.OK, scope.StatusCode);
        var lecturer = await SqlFactory.Create(admin);
        var m = await f.Login(manager.UserName);
        var l = await f.Login(lecturer.UserName);
        var code = "P" + Key()[..10];
        var created = await m.PostAsJsonAsync("/api/v1/rooms", new SaveRoom(code, "Phòng kiểm thử " + code, "Phòng học", "administration", 50, true, null));
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        return (m, l, (await created.Content.ReadFromJsonAsync<RoomView>())!);
    }

    private static async Task<RoomRequestView> Draft(HttpClient lecturer, RoomView room, int start = 8, int end = 10)
    {
        var response = await Send(lecturer, "/api/v1/room-requests", new SaveRoomRequest(room.Id, Tomorrow(start), Tomorrow(end), 25, "", null));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<RoomRequestView>())!;
    }

    [Fact]
    public async Task DraftDoesNotHoldSlotAndCreateAndSubmitAreIdempotent()
    {
        var (m, l, room) = await Setup(); using var manager = m; using var lecturer = l;
        var key = Key();
        var input = new SaveRoomRequest(room.Id, Tomorrow(8), Tomorrow(10), 25, null, null);
        var first = await Send(l, "/api/v1/room-requests", input, key);
        var r = (await first.Content.ReadFromJsonAsync<RoomRequestView>())!;
        var replay = (await (await Send(l, "/api/v1/room-requests", input, key)).Content.ReadFromJsonAsync<RoomRequestView>())!;
        Assert.Equal(r.Id, replay.Id);
        Assert.Equal(HttpStatusCode.Conflict, (await Send(l, "/api/v1/room-requests", input with { Attendees = 26 }, key)).StatusCode);
        var date = DateOnly.FromDateTime(Tomorrow(8).DateTime).ToString("yyyy-MM-dd");
        var before = await l.GetFromJsonAsync<JsonElement>($"/api/v1/rooms/{room.Id}/availability?date={date}");
        Assert.Single(before.GetProperty("slots").EnumerateArray());
        var actionKey = Key();
        var action = new RoomAction(r.Version, null);
        var submitted = await Send(l, $"/api/v1/room-requests/{r.Id}/submit", action, actionKey);
        Assert.Equal(HttpStatusCode.OK, submitted.StatusCode);
        Assert.Equal("pending", (await submitted.Content.ReadFromJsonAsync<RoomRequestView>())!.Status);
        Assert.Equal(HttpStatusCode.OK, (await Send(l, $"/api/v1/room-requests/{r.Id}/submit", action, actionKey)).StatusCode);
        await using var scope = f.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        Assert.Equal(1, await db.RoomReservations.CountAsync(x => x.RequestId == r.Id && x.Active));
        Assert.Equal(1, await db.AuditEvents.CountAsync(x => x.TargetId == r.Id && x.Action == "room-request.submit"));
    }

    [Fact]
    public async Task ConcurrentOverlappingSubmissionsCommitExactlyOneReservation()
    {
        var (m, l, room) = await Setup(); using var manager = m; using var lecturer = l;
        var drafts = new List<RoomRequestView>();
        for (var i = 0; i < 12; i++) drafts.Add(await Draft(l, room));
        var results = await Task.WhenAll(drafts.Select(r => Send(l, $"/api/v1/room-requests/{r.Id}/submit", new RoomAction(r.Version, null))));
        Assert.Single(results, x => x.StatusCode == HttpStatusCode.OK);
        Assert.Equal(11, results.Count(x => x.StatusCode == HttpStatusCode.Conflict));
        await using var scope = f.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HpuniDbContext>();
        Assert.Equal(1, await db.RoomReservations.CountAsync(x => x.RoomId == room.Id && x.Active));
        Assert.Equal(1, await db.RoomRequests.CountAsync(x => x.RoomId == room.Id && x.Status == "pending"));
    }

    [Fact]
    public async Task AdjacentSlotsAllowedAndCancellationReleasesOnlyItsOwnSlot()
    {
        var (m, l, room) = await Setup(); using var manager = m; using var lecturer = l;
        var a = await Draft(l, room, 8, 10); var b = await Draft(l, room, 10, 11);
        a = (await (await Send(l, $"/api/v1/room-requests/{a.Id}/submit", new RoomAction(a.Version, null))).Content.ReadFromJsonAsync<RoomRequestView>())!;
        b = (await (await Send(l, $"/api/v1/room-requests/{b.Id}/submit", new RoomAction(b.Version, null))).Content.ReadFromJsonAsync<RoomRequestView>())!;
        Assert.Equal("pending", b.Status);
        Assert.Equal(HttpStatusCode.OK, (await Send(l, $"/api/v1/room-requests/{a.Id}/cancel", new RoomAction(a.Version, null))).StatusCode);
        var date = DateOnly.FromDateTime(Tomorrow(8).DateTime).ToString("yyyy-MM-dd");
        var availability = await l.GetFromJsonAsync<JsonElement>($"/api/v1/rooms/{room.Id}/availability?date={date}");
        var slots = availability.GetProperty("slots").EnumerateArray().ToArray();
        Assert.Equal(2, slots.Length);
        Assert.Equal(Tomorrow(10), slots[0].GetProperty("end").GetDateTimeOffset());
        var approved = await Send(m, $"/api/v1/room-requests/{b.Id}/approve", new RoomAction(b.Version, ""));
        Assert.Equal(HttpStatusCode.OK, approved.StatusCode);
        b = (await approved.Content.ReadFromJsonAsync<RoomRequestView>())!;
        Assert.Equal(HttpStatusCode.BadRequest, (await Send(l, $"/api/v1/room-requests/{b.Id}/cancel", new RoomAction(b.Version, null))).StatusCode);
    }

    [Fact]
    public async Task MaintenanceAndCapacityAreRecheckedAndFailedActionCanRetry()
    {
        var (m, l, room) = await Setup(); using var manager = m; using var lecturer = l;
        var r = await Draft(l, room);
        var blockResponse = await Send(m, $"/api/v1/rooms/{room.Id}/blocks", new SaveRoomBlock(Tomorrow(9), Tomorrow(11), null));
        Assert.Equal(HttpStatusCode.OK, blockResponse.StatusCode);
        var block = (await blockResponse.Content.ReadFromJsonAsync<RoomBlockView>())!;
        var actionKey = Key();
        Assert.Equal(HttpStatusCode.Conflict, (await Send(l, $"/api/v1/room-requests/{r.Id}/submit", new RoomAction(r.Version, null), actionKey)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Send(m, $"/api/v1/room-blocks/{block.Id}/cancel", new RoomAction(block.Version, null))).StatusCode);
        var retry = await Send(l, $"/api/v1/room-requests/{r.Id}/submit", new RoomAction(r.Version, null), actionKey);
        Assert.Equal(HttpStatusCode.OK, retry.StatusCode); // Failed transaction did not consume the key or write a reservation.
        Assert.Equal(HttpStatusCode.BadRequest, (await m.PutAsJsonAsync($"/api/v1/rooms/{room.Id}",
            new SaveRoom(room.Code, room.Name, room.Type, room.DepartmentId, 10, true, room.Version))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await m.PutAsJsonAsync($"/api/v1/rooms/{room.Id}",
            new SaveRoom(room.Code, room.Name, room.Type, room.DepartmentId, 50, false, room.Version))).StatusCode);
    }

    [Fact]
    public async Task AnotherLecturerCannotReadOrChangeRequestsAndMissingIdempotencyIsRejected()
    {
        var (m, l, room) = await Setup(); using var manager = m; using var lecturer = l;
        var r = await Draft(l, room);
        using var admin = await f.Login();
        var another = await SqlFactory.Create(admin);
        using var other = await f.Login(another.UserName);
        Assert.Equal(HttpStatusCode.NotFound, (await other.GetAsync($"/api/v1/room-requests/{r.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Send(other, $"/api/v1/room-requests/{r.Id}/submit", new RoomAction(r.Version, null))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await l.PostAsJsonAsync("/api/v1/room-requests",
            new SaveRoomRequest(room.Id, Tomorrow(8), Tomorrow(10), 25, null, null))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Send(l, "/api/v1/room-requests",
            new SaveRoomRequest(room.Id, Tomorrow(8), Tomorrow(10), 51, null, null))).StatusCode);
    }
}
