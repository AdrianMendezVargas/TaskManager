using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Models.Domain;
using TaskManager.Models.Mappers;
using TaskManager.Repository;
using TaskManager.Shared;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Services {
    public class UserService : BaseService, IUserService {
        private readonly IUnitOfWork _unit;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unit, IConfiguration configuracion) {
            _unit = unit;
            _configuration = configuracion;
        }

        public async Task<OperationResponse<TokenResponse>> RegisterUserAsync(RegisterUserRequest request) {

            if (!request.IsValid()) {
                return Error("Invalid request", new TokenResponse());
            }

            bool IsAlreadyTaken = _unit.UserRepository.FindUserByEmailAsync(request.Email) != null;
            if (IsAlreadyTaken) {
                return Error("This user is already taken" , new TokenResponse());
            }

            var user = ApplicationUserMapper.ToApplicationUser(request);
            user.Password = Utilities.GetSHA256(user.Password);
            
            await _unit.UserRepository.CreateUserAsync(user);
            bool done = await _unit.CommitChangesAsync();

            if (done) {
                return Success("User was successfully registered", BuildToken(user));
            } else {
                return Error("Could not create the user" , new TokenResponse());
            }

        }

        public async Task<OperationResponse<TokenResponse>> LoginUserAsync(LoginRequest credencials) {
            if (!credencials.IsValid()) {
                return Error("Invalid credentials" , new TokenResponse());
            }

            var user = await _unit.UserRepository.FindUserByCredentialsAsync(credencials);
            return user == null ? Error("Invalid credentials" , new TokenResponse())
                                : Success("Successfully logged in", BuildToken(user));
        }

        public async Task<OperationResponse<ApplicationUser>> UpdateUserAsync(ApplicationUser user) {
            return null;
        }

        private TokenResponse BuildToken(ApplicationUser user) {
            if (!user.IsValid()) {
                return new TokenResponse();
            }
            //Setting up the claims
            var claims = new[] {
               new Claim(ClaimTypes.Email, user.Email),
               new Claim(ClaimTypes.Role, "User"),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Jti is the unique identifier of de Token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtKey").Value));  //Creating the Jwt Signature Key. JwtKey is an environment variable
            var credencials = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);   //Creating the credentials using the key and the specified algorithm
            var expirationDate = DateTime.UtcNow.AddHours(2); // Expiration time of the token

            JwtSecurityToken token = new JwtSecurityToken(   //Setting up the token data
               issuer: "yourdomain.com" ,
               audience: "yourdomain.com" ,
               claims: claims ,
               expires: expirationDate ,
               signingCredentials: credencials);

            return new TokenResponse {
                Token = new JwtSecurityTokenHandler().WriteToken(token) ,   // Writing the token
                ExpirationDate = expirationDate
            };

        }

    }
}
