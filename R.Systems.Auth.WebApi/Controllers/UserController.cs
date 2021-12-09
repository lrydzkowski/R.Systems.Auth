﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R.Systems.Auth.Core.Models;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.SharedKernel.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace R.Systems.Auth.WebApi.Controllers
{
    [ApiController, Route("users")]
    public class UserController : ControllerBase
    {
        public UserController(
            UserReadService userReadService,
            UserWriteService userWriteService,
            ValidationResult validationResult)
        {
            UserReadService = userReadService;
            UserWriteService = userWriteService;
            ValidationResult = validationResult;
        }

        public UserReadService UserReadService { get; }
        public UserWriteService UserWriteService { get; }
        public ValidationResult ValidationResult { get; }

        [HttpGet, Route("{userId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Get(long userId)
        {
            UserDto? user = await UserReadService.GetAsync(userId);
            if (user == null)
            {
                return NotFound(null);
            }
            return Ok(user);
        }

        [HttpGet, Authorize(Roles = "admin")]
        public async Task<IActionResult> Get()
        {
            List<UserDto> users = await UserReadService.GetAsync();
            return Ok(users);
        }

        [HttpPost, Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(EditUserDto editUserDto)
        {
            bool result = await UserWriteService.EditUserAsync(editUserDto);
            if (!result)
            {
                return BadRequest(ValidationResult.Errors);
            }
            return Ok();
        }

        [HttpPost, Route("{userId}"), Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(EditUserDto editUserDto, long userId)
        {
            bool result = await UserWriteService.EditUserAsync(editUserDto, userId);
            if (!result)
            {
                return BadRequest(ValidationResult.Errors);
            }
            return Ok();
        }
    }
}
