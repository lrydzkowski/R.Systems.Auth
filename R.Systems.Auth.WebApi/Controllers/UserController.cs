using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.WebApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.Controllers
{
    [ApiController, Route("users")]
    public class UserController : ControllerBase
    {
        public UserController(UserService userService)
        {
            UserService = userService;
        }

        public UserService UserService { get; }

        [HttpGet, Route("{userId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Get(long userId)
        {
            User? user = await UserService.GetUserAsync(userId);
            return Ok(new
            {
                Data = user
            });
        }

        [HttpGet, Authorize(Roles = "admin")]
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
