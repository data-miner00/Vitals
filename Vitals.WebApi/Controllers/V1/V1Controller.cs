namespace Vitals.WebApi.Controllers.V1;

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public abstract class V1Controller : ControllerBase
{
    /// <summary>
    /// Gets the cancellation token of the current HTTP request.
    /// </summary>
    public CancellationToken CancellationToken => this.HttpContext.RequestAborted;
}
