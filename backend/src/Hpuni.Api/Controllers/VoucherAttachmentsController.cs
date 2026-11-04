using System.Security.Claims;
using Hpuni.Application;
using Hpuni.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hpuni.Api.Controllers;

[ApiController,Route("api/v1"),Authorize]
public class VoucherAttachmentsController(VoucherAttachmentService service):ControllerBase
{
    string Actor=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet("finance/entries/{entryId}/attachments")]
    public async Task<IActionResult> List(string entryId,CancellationToken ct)=>Ok(await service.List(Actor,entryId,ct));
    [HttpPost("finance/entries/{entryId}/attachments"),RequestSizeLimit(3*1024*1024),RequestFormLimits(MultipartBodyLengthLimit=3*1024*1024)]
    public async Task<IActionResult> Upload(string entryId,[FromForm]IFormFile file,CancellationToken ct)
    {
        if(file.Length is <1 or >VoucherAttachmentService.MaxBytes)throw AppException.Invalid("Tệp phải từ 1 byte đến 2 MiB.");
        using var buffer=new MemoryStream();await file.CopyToAsync(buffer,ct);
        return Ok(await service.Upload(Actor,entryId,Request.Headers["Idempotency-Key"].ToString(),file.FileName,buffer.ToArray(),ct));
    }
    [HttpGet("voucher-attachments/{id}")]
    public async Task<IActionResult> Download(string id,CancellationToken ct)
    {
        var file=await service.Download(Actor,id,ct);
        Response.Headers.ContentSecurityPolicy="sandbox; default-src 'none'";
        return File(file.Content,file.Metadata.ContentType,file.Metadata.FileName);
    }
}
