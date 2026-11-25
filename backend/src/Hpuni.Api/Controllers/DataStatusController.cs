using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hpuni.Api.Controllers;

[ApiController, Route("api/v1/data-status"), Authorize]
public class DataStatusController(HpuniDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(new
    {
        hasSampleData = await db.AuditEvents.AnyAsync(x => x.Action == "sample-data.add", ct),
        sampleMarker = "[MẪU]"
    });
}
