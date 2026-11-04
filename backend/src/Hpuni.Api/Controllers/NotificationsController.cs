using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hpuni.Api.Controllers;

[ApiController,Route("api/v1/notifications"),Authorize]
public class NotificationsController(NotificationsService service):ControllerBase
{
    string Actor=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet]
    public Task<NotificationPage> List([FromQuery]int page=1,[FromQuery]int pageSize=20,[FromQuery]bool unreadOnly=false,CancellationToken ct=default)=>service.List(Actor,page,pageSize,unreadOnly,ct);
    [HttpPost("{id}/read")]
    public async Task<IActionResult> Read(string id,CancellationToken ct){await service.Read(Actor,id,ct);return NoContent();}
}
