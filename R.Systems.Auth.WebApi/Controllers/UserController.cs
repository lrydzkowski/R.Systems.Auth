using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Services;
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
            UserDto? user = await UserService.GetAsync(userId);
            return Ok(user);
        }

        [HttpGet, Authorize(Roles = "admin")]
        public async Task<IActionResult> Get()
        {
            List<UserDto> users = await UserService.GetAsync();
            return Ok(users);
        }
    }
}
