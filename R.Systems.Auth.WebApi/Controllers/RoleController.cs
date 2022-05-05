using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Core.Models.Roles;
using R.Systems.Auth.Core.Services;

namespace R.Systems.Auth.WebApi.Controllers;

[ApiController, Route("roles")]
public class RoleController : ControllerBase
{
    public RoleController(RoleService roleService)
    {
        RoleService = roleService;
    }

    public RoleService RoleService { get; }

    [HttpGet, Authorize(Roles = "admin")]
    public async Task<IActionResult> GetRoles()
    {
        List<RoleDto> roles = await RoleService.GetAsync();
        return Ok(roles);
    }
}
