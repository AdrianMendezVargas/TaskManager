using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Services;
using TaskManager.Shared;
using TaskManager.Shared.Requests;

namespace TaskManager.Api.Controllers {

    [Produces("application/json")]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase {
        private readonly IUserService _userService;

        public AccountController(IUserService userService) {
            _userService = userService;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest model) {

            if (ModelState.IsValid) {
                var result = await _userService.RegisterUserAsync(model);
                if (result.IsSuccess) {
                    return Ok(result);
                } else {
                    return BadRequest(result);
                }
            } else {
                return BadRequest(ModelState);
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest credencials) {
            if (ModelState.IsValid) {
                var result = await _userService.LoginUserAsync(credencials);
                if (result.IsSuccess) {
                    return Ok(result);
                } else {
                    ModelState.AddModelError(string.Empty , "Invalid login attempt.");
                    return BadRequest(result);
                }
            } else {
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route("token")]
        public IActionResult IsTokenValid([FromBody] GetClaimsRequest request) {
            if (ModelState.IsValid) {
                var result = _userService.GetClaimsFromToken(request.Token);
                if (result.IsSuccess) {
                    return Ok(result);
                } else {
                    return BadRequest(result);
                }
            } else {
                return BadRequest(ModelState);
            }
        }


    }
}
