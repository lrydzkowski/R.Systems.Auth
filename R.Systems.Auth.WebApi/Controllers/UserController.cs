using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.WebApi.Features.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.Controllers
{
    [ApiController, Route("users")]
    public class UserController : ControllerBase
    {
        public UserController(GetUsersHandler getUsersHandler)
        {
            GetUsersHandler = getUsersHandler;
        }

        public GetUsersHandler GetUsersHandler { get; }

        [HttpGet, Route("{userId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Get(long userId)
        {
            User? user = await GetUsersHandler.HandleAsync(userId);
            return Ok(new
            {
                Data = user
            });
        }

        [HttpGet, Authorize(Roles = "admin")]
        public async Task<IActionResult> Get()
        {
            List<User> users = await GetUsersHandler.HandleAsync();
            return Ok(new
            {
                Data = users
            });
        }
    }
}
