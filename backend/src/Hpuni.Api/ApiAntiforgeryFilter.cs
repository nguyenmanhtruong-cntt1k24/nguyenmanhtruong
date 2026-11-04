using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hpuni.Api;

// Global API equivalent of AutoValidateAntiforgeryToken without the MVC ViewFeatures dependency.
public class ApiAntiforgeryFilter(IAntiforgery antiforgery) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var method = context.HttpContext.Request.Method;
        if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method) || HttpMethods.IsTrace(method)) return;
        try { await antiforgery.ValidateRequestAsync(context.HttpContext); }
        catch (AntiforgeryValidationException)
        {
            var problem = new ProblemDetails { Status = 400, Title = "Phiên biểu mẫu không hợp lệ. Vui lòng tải lại." };
            problem.Extensions["code"] = "CSRF_INVALID";
            problem.Extensions["request_id"] = context.HttpContext.TraceIdentifier;
            context.Result = new ObjectResult(problem) { StatusCode = 400 };
        }
    }
}
