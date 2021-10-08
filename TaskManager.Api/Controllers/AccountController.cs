using Microsoft.AspNetCore.Authorization;
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
using TaskManager.Shared.Enums;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Api.Controllers {

    [Produces("application/json")]
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase {
        private readonly IUserService _userService;

        public AccountController(IUserService userService) {
            _userService = userService;
        }

        [ProducesResponseType(200 , Type = typeof(OperationResponse<TokenResponse>))]
        [ProducesResponseType(400 , Type = typeof(EmptyOperationResponse))]
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest model) {

            if (ModelState.IsValid) {
                var result = await _userService.RegisterUserAsync(model);
                if (result.IsSuccess) {
                    return Ok(result);
                } else {
                    return BadRequest(new EmptyOperationResponse{ 
                        IsSuccess = false,
                        Message = result.Message
                    });
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

        [Authorize(Roles = UserRoles.NotVerifiedUser)]
        [HttpPost]
        [Route("emailValidation")]
        public async Task<IActionResult> ValidateEmail([FromBody] EmailVerificationRequest request) {
            if (ModelState.IsValid) {
                var result = await _userService.ValidateAccountRecoveryCodeAsync(request);
                if (result.IsSuccess) {
                    return Ok(result);
                } else {
                    return BadRequest(new {
                        Message = result.Message,
                        IsSuccess = result.IsSuccess
                    });
                }
            } else {
                return BadRequest(ModelState);
            }
        }

        [Authorize(Roles = UserRoles.NotVerifiedUser)]
        [HttpGet]
        [Route("resendEmailValidation")]
        public async Task<IActionResult> ResendEmailVerification() {
                var result = await _userService.ResendAccountVerificationEmail();
                return Ok(result);
        }

        [HttpPost]
        [Route("token")]
        public IActionResult GetClaimsFromToken([FromBody] GetClaimsRequest request) {
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

        [ProducesResponseType(200 , Type = typeof(OperationResponse<TokenResponse>))]
        [ProducesResponseType(400 , Type = typeof(EmptyOperationResponse))]
        [Authorize]
        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> GetTokenFromRefreshToken(RefreshTokenRequest refreshTokenRequest) {
            var result = await _userService.GetNewTokenFromRefreshToken(refreshTokenRequest);
            if (result.IsSuccess) {
                return Ok(result);
            } else {
                return BadRequest(result);
            }
        }


    }
}
