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
using TaskManager.Shared;
using TaskManager.Shared.Requests;

namespace TaskManager.Api.Controllers {

    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager, 
                                IConfiguration configuration) {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserInfo model) {

            if (ModelState.IsValid) {
                var user = new ApplicationUser { UserName = model.Email , Email = model.Email };
                var result = await _userManager.CreateAsync(user , model.Password);
                if (result.Succeeded) {
                    return BuildToken(model);
                } else {
                    return BadRequest("Username or password invalid");
                }
            } else {
                return BadRequest(ModelState);
            }

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserInfo userInfo) {
            if (ModelState.IsValid) {
                var result = await _signInManager.PasswordSignInAsync(userInfo.Email , userInfo.Password , isPersistent: false , lockoutOnFailure: false);
                if (result.Succeeded) {
                    return BuildToken(userInfo);
                } else {
                    ModelState.AddModelError(string.Empty , "Invalid login attempt.");
                    return BadRequest(ModelState);
                }
            } else {
                return BadRequest(ModelState);
            }
        }

        private IActionResult BuildToken(UserInfo userInfo) {
            var claims = new[] {
               new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
               new Claim("miValor", "Lo que yo quiera"),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Jti is the unique identifier of de Token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));  //Creating the Jwt Signature Key. JwtKey is an environment variable
            var credencials = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);   //Creating the credentials using the key and the specified algorithm

            var expiration = DateTime.UtcNow.AddHours(2); // Expiration time of the token

            JwtSecurityToken token = new JwtSecurityToken(   //Setting up the token data
               issuer: "yourdomain.com" ,
               audience: "yourdomain.com" ,
               claims: claims ,
               expires: expiration ,
               signingCredentials: credencials);

            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token) ,   // Writing the token
                expiration = expiration
            });

        }
    }
}
