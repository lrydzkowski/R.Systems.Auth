using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Models;
using R.Systems.Auth.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Controllers
{
    [ApiController, Route("users")]
    public class UserController : ControllerBase
    {
        public UserController(UserService userService)
        {
            UserService = userService;
        }

        public UserService UserService { get; }

        [HttpPost, Route("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest request)
        {
            string? jwtToken = await UserService.AuthenticateAsync(request);
            if (jwtToken == null)
            {
                return Unauthorized();
            }
            return Ok(new AuthenticateResponse
            {
                AccessToken = jwtToken
            });
        }

        [HttpGet, Route("{userId}")]
        public async Task<IActionResult> Get(long userId)
        {
            User? user = await UserService.GetUserAsync(userId);
            return Ok(new
            {
                Data = user
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<User> users = await UserService.GetUsersAsync();
            return Ok(new
            {
                Data = users
            });
        }
    }
}
