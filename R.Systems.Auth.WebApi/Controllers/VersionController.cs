using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.WebApi.Features.Version;

namespace R.Systems.Auth.WebApi.Controllers;

[ApiController]
public class VersionController : ControllerBase
{
    public VersionController(GetVersionHandler getVersionHandler)
    {
        GetVersionHandler = getVersionHandler;
    }

    public GetVersionHandler GetVersionHandler { get; }

    [HttpGet, Route("version")]
    public IActionResult GetVersion()
    {
        GetVersionResponse response = new()
        {
            Version = GetVersionHandler.Handle()
        };
        return Ok(response);
    }
}
