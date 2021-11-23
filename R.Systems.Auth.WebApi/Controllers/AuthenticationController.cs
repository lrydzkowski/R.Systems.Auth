using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.WebApi.Features.Authentication;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.Controllers
{
    [ApiController, Route("users")]
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(AuthenticateHandler authenticateHandler)
        {
            AuthenticateHandler = authenticateHandler;
        }

        public AuthenticateHandler AuthenticateHandler { get; }

        [HttpPost, Route("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest request)
        {
            AuthenticateResponse? response = await AuthenticateHandler.HandleAsync(request);
            if (response == null)
            {
                return Unauthorized();
            }
            return Ok(response);
        }
    }
}
