using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Common.Models;
using R.Systems.Auth.Common.Repositories;
using R.Systems.Auth.Models;
using R.Systems.Auth.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.Controllers
{
    [ApiController, Route("users")]
    public class UserController : ControllerBase
    {
        public UserController(IRepository<User> repository, AuthenticationService authenticationService)
        {
            Repository = repository;
            AuthenticationService = authenticationService;
        }

        public IRepository<User> Repository { get; }
        public AuthenticationService AuthenticationService { get; }

        [HttpPost, Route("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest request)
        {
            bool isLogged = await AuthenticationService.AuthenticateAsync(request.Email, request.Password);
            if (!isLogged) return Unauthorized();
            string? jwtToken = await AuthenticationService.GenerateJwtTokenAsync(request.Email);
            if (jwtToken == null) return Unauthorized();
            return Ok(new
            {
                Data = jwtToken
            });
        }

        [HttpGet, Route("{userId}")]
        public async Task<IActionResult> Get(long userId)
        {
            User? user = await Repository.GetAsync(userId);
            return Ok(new
            {
                Data = user
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<User> users = await Repository.GetAsync();
            return Ok(new
            {
                Data = users
            });
        }
    }
}
