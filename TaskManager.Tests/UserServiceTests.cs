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
using System.Net.Mail;
using System.Net;
using TaskManager.Shared.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManager.Tests.MockModels;
using TaskManager.Shared.Enums;

namespace TaskManager.Tests {
    [TestClass()]
    public class UserServiceTests {
        private const string userToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJlbGFkcmktQGxpdmUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImp0aSI6ImZhZDYzYmFlLTU4ZTItNDk2Yy1iM2U0LTczZGQxM2MxZWE0MiIsImlzcyI6InlvdXJkb21haW4uY29tIiwiYXVkIjoieW91cmRvbWFpbi5jb20ifQ.JWFhxzQG6jqVkNPN6dYjBDtzPE9UZFMsG6LvWViCwFQ";
        private const string secretKey = "ClaveSuperSecretAZNM82HDY7Y1PSKCMX9JD712JSGH";
        private UserService UserService;
        private IUnitOfWork Unit;

        [TestInitialize]
        public void Initialize() {
            var db = DbContextHelper.GetSeedInMemoryDbContext();
            Unit = new EfUnitOfWork(db);

            var configuration = new Mock<IConfiguration>();
            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(x => x.Value).Returns(secretKey);
            configuration.Setup(x => x.GetSection("JwtKey")).Returns(configSection.Object);

            var principal = GetPrincipalFromToken(userToken);
            var httpContext = new FakeHttpContext(principal);
            var httpContextAccessor = new FakeHttpContextAccessor(httpContext);

            SmtpClient smtpClient = new SmtpClient {
                Host = "smtp-mail.outlook.com" ,
                Port = 587 ,
                EnableSsl = true ,
                Credentials = new NetworkCredential("" , "")
            };

            var mailService = new MailService(smtpClient);

            UserService = new UserService(Unit , configuration.Object , httpContextAccessor , mailService);
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
            var principal = GetPrincipalFromToken(token);
            return principal.Identity.IsAuthenticated;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token) {
            TokenValidationParameters validationParameters = getValidationParamerers();
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token , validationParameters , out _);
            return principal;
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
                ValidateLifetime = false ,
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

        [TestMethod()]
        public async Task SendAccountVerificationEmail_InvalidEmail_ShouldReturnAfailedResponse() {
            var response = await UserService.SendAccountVerificationEmail("laksdjflajf");
            Assert.IsTrue(!response.IsSuccess);
        }

        //[TestMethod()]
        public async Task SendAccountVerificationEmail_validEmail_ShouldSaveAnEmailVerificationAndReturnSuccessResponse() {
            int userId = 1;
            string userEmail = "eladri-@live.com";
            int expectedNewEmailVerificationId = 4;

            var response = await UserService.SendAccountVerificationEmail(userEmail);

            var emailVerification = await Unit.EmailVerificationRepository.FindLatestByUserId(userId);
            Assert.IsTrue(response.IsSuccess);
            Assert.IsTrue(emailVerification.Id == expectedNewEmailVerificationId);
        }

        [TestMethod()]
        public async Task ValidateAccountRecoveryPin_ValidRecoveryPin_ShoultSetTheUserAsVerifiedUser() {
            int validPin = 11111;
            int userId = 1;

            var response = await UserService.ValidateAccountRecoveryPinAsync(validPin);

            var user = await Unit.UserRepository.FindUserByIdAsync(userId);
            Assert.IsTrue(response.IsSuccess, $"Operation failed: {response.Message}");
            Assert.IsTrue(user.Role == UserRoles.VerifiedUser, "The user role did not change to verified");
        }

        [TestMethod()]
        public async Task ValidateAccountRecoveryPin_NotLatestValidRecoveryPin_ShoultSetTheUserAsVerifiedUser() {
            int validPin = 44444;
            int userId = 1;

            var response = await UserService.ValidateAccountRecoveryPinAsync(validPin);

            var user = await Unit.UserRepository.FindUserByIdAsync(userId);
            Assert.IsTrue(!response.IsSuccess);
            Assert.IsTrue(user.Role == UserRoles.NotVerifiedUser);
        }

    }
}