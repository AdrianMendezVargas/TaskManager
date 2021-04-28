using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Repository;
using TaskManager.Models.Domain;
using TaskManager.Shared.Requests;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Microsoft.Extensions.Configuration;

namespace TaskManager.Tests {
    [TestClass()]
    public class UserServiceTests {
        private const string secretKey = "ClaveSuperSecretAZNM82HDY7Y1PSKCMX9JD712JSGH";
        private UserService UserService;

        [TestInitialize]
        public void Initialize() {
            var db = DbContextHelper.GetSeedInMemoryDbContext();
            var unit = new EfUnitOfWork(db);

            var configuration = new Mock<IConfiguration>();
            var configSection = new Mock<IConfigurationSection>();

            configSection.Setup(x => x.Value).Returns(secretKey);
            configuration.Setup(x => x.GetSection("JwtKey")).Returns(configSection.Object);

            UserService = new UserService(unit , configuration.Object);
        }

        [TestMethod()]
        public async Task CreateUser_ShouldRegisterTheUserAndReturnValidBearerToken() {
            var user = new RegisterUserRequest {
                Email = "user@gmail.com" ,
                Password = "Aa123456" ,
            };

            var result = await UserService.RegisterUserAsync(user);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(IsTokenValid(result.Record.Token));

        }

        [TestMethod()]
        public async Task CreateUser_AlreadyTaken_ShouldNotRegisterTheUser() {
            var user = new RegisterUserRequest {
                Email = "eladri-@live.com" ,
                Password = "Aa123456" ,
            };

            var result = await UserService.RegisterUserAsync(user);

            Assert.IsTrue(!result.IsSuccess);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Record.Token));

        }

        public bool IsTokenValid(string token) {
            SecurityToken securityToken;
            TokenValidationParameters validationParameters = getValidationParamerers();
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token , validationParameters , out securityToken);
            return principal.Identity.IsAuthenticated;
        }

        [TestMethod()]
        public async Task CreateUser_EmptyEmail_ShoudNotRegisterTheUserAndReturnANullTokenResponce() {
            var user = new RegisterUserRequest {
                Email = "" ,
                Password = "Aa123456" ,
            };

            var result = await UserService.RegisterUserAsync(user);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Record.Token));
        }

        [TestMethod()]
        public async Task CreateUser_EmptyPassword_ShoudNotRegisterTheUserAndReturnANullTokenResponce() {
            var user = new RegisterUserRequest {
                Email = "correo@gmail.com" ,
                Password = "" ,
            };

            var result = await UserService.RegisterUserAsync(user);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Record.Token));
        }


        private static TokenValidationParameters getValidationParamerers() {
            return new TokenValidationParameters {
                ValidateIssuer = true ,
                ValidateAudience = true ,
                ValidateLifetime = true ,
                ValidateIssuerSigningKey = true ,
                ValidIssuer = "yourdomain.com" ,
                ValidAudience = "yourdomain.com" ,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) ,
                ClockSkew = TimeSpan.Zero
            };
        }

        [TestMethod()]
        public async Task LoginUser_ValidCredentials_SholudReturnAValidTokenResponse() {
            var credentials = new LoginRequest {
                Email = "eladri-@live.com" ,
                Password = "clave" ,
            };

            var result = await UserService.LoginUserAsync(credentials);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(IsTokenValid(result.Record.Token));
        }

        [TestMethod()]
        public async Task LoginUser_InvalidCredentials_SholudReturnAnEmptyToken() {
            var credentials = new LoginRequest {
                Email = "eladri-@live.com" ,
                Password = "incorrecta" ,
            };

            var result = await UserService.LoginUserAsync(credentials);

            Assert.IsTrue(!result.IsSuccess);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Record.Token));
        }
    }
}