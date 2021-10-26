using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Models;
using System.Reflection;

namespace R.Systems.Auth.Controllers
{
    [ApiController]
    public class VersionController : ControllerBase
    {
        [HttpGet, Route("version")]
        public IActionResult Get()
        { 
            string version = Assembly.GetExecutingAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                .ToString() ?? "";
            return Ok(new VersionResponse
            {
                Version = version
            });
        }
    }
}
