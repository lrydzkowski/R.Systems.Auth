using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Models;
using R.Systems.Auth.Services;
using System.Threading.Tasks;

namespace R.Systems.Auth.Controllers
{
    [ApiController, Route("users")]
    public class AuthenticateController : ControllerBase
    {
        public AuthenticateController(AuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public AuthenticationService AuthenticationService { get; }

        [HttpPost, Route("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest request)
        {
            string? jwtToken = await AuthenticationService.AuthenticateAsync(request);
            if (jwtToken == null)
            {
                return Unauthorized();
            }
            return Ok(new AuthenticateResponse
            {
                AccessToken = jwtToken
            });
        }
    }
}
