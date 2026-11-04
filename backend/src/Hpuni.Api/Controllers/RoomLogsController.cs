using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hpuni.Api.Controllers;

[ApiController, Authorize, Route("api/v1/room-requests/{requestId}/log")]
public class RoomLogsController(RoomLogService service) : ControllerBase
{
    private string Actor => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet]
    public async Task<IActionResult> Get(string requestId, [FromQuery] bool review = false, CancellationToken ct = default) =>
        Ok(await service.Get(Actor, requestId, review, ct));
    [HttpPut]
    public async Task<IActionResult> Submit(string requestId, SaveRoomLog input, CancellationToken ct) =>
        Ok(await service.Submit(Actor, requestId, input, ct));
    [HttpPost("review")]
    public async Task<IActionResult> Review(string requestId, ReviewRoomLog input, CancellationToken ct) =>
        Ok(await service.Review(Actor, requestId, input, ct));
}
