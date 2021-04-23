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
            var db = DbContextHelper.GetSeedDbContext();
            var unit = new EfUnitOfWork(db);

            var configuration = new Mock<IConfiguration>();
            var configSection = new Mock<IConfigurationSection>();

            configSection.Setup(x => x.Value).Returns(secretKey);
            configuration.Setup(x => x.GetSection("JwtKey")).Returns(configSection.Object);

            UserService = new UserService(unit, configuration.Object);
        }

        [TestMethod()]
        public async Task CreateUser_ShouldRegisterTheUserAndReturnValidBearerToken() {
            var user = new RegisterUserRequest {
                Email = "user@gmail.com" ,
                Password = "Aa123456" ,
            };

            SecurityToken securityToken;
            TokenValidationParameters validationParameters = getValidationParamerers();
            var tokenHandler = new JwtSecurityTokenHandler();

            var result = await UserService.RegisterUserAsync(user);
            var principal = tokenHandler.ValidateToken(result.Record.Token , validationParameters , out securityToken);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(principal);

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
    }
}