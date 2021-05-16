using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Models.Domain;
using TaskManager.Models.Mappers;
using TaskManager.Repository;
using TaskManager.Shared;
using TaskManager.Shared.Enums;
using TaskManager.Shared.Extensions;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;
using TaskManager.Shared.Services;

namespace TaskManager.Services {
    public class UserService : BaseService, IUserService {
        private const int PIN_LENGTH = 5;
        private const int PIN_EXPIRATION_HOURS = 2;

        private readonly IUnitOfWork _unit;
        private readonly IConfiguration _configuration;
        private readonly HttpContext _httpContext;
        private readonly IMailService _mailService;

        //TODO: Create an Account service to separate functionality
        public UserService(IUnitOfWork unit, IConfiguration configuracion, IHttpContextAccessor contextAccessor, IMailService mailService) {
            _unit = unit;
            _configuration = configuracion;
            _httpContext = contextAccessor.HttpContext;
            _mailService = mailService;
        }

        public async Task<OperationResponse<TokenResponse>> RegisterUserAsync(RegisterUserRequest request) {

            if (!request.IsValid()) {
                return Error("Invalid request", new TokenResponse());
            }

            bool IsAlreadyTaken = await _unit.UserRepository.FindUserByEmailAsync(request.Email) != null;
            if (IsAlreadyTaken) {
                return Error("This email is already taken" , new TokenResponse());
            }

            var user = ApplicationUserMapper.ToApplicationUser(request);
            user.Password = Utilities.GetSHA256(user.Password);
            
            await _unit.UserRepository.CreateUserAsync(user);     
            bool done = await _unit.CommitChangesAsync();

            if (done) {
                await SendAccountVerificationEmail(request.Email);
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

        public Task<OperationResponse<ApplicationUser>> UpdateUserAsync(ApplicationUser user) {
            throw new NotImplementedException();
        }

        public async Task<OperationResponse<TokenResponse>> ValidateAccountRecoveryPinAsync(EmailVerificationRequest verificationRequest) {
             //TODO: Change the responses messages

            //TODO: Fix the code generator
            //if (verificationRequest.Code.ToString().Length < PIN_LENGTH) {
            //    return Error("This code is invalid" , new TokenResponse { });
            //}

            int principalId = GetUserIdFromPrincipal();
            var lastEmailVerification = await _unit.EmailVerificationRepository.FindLatestByUserId(principalId);

            if (lastEmailVerification == null) {
                return Error("You don't have any recovery code sent" , new TokenResponse { }); 
            }

            if (verificationRequest.Code != lastEmailVerification.RecoveryCode) {
                return Error("This code does not match" , new TokenResponse { });
            }

            if (lastEmailVerification.WasValidated) {
                return Error("This code was already used by you" , new TokenResponse { });
            }

            if (lastEmailVerification.IsExpired()) {
                return Error("This code has expired" , new TokenResponse { });
            }

            SetEmailVerificationAsValidated(lastEmailVerification);
            var tokenResponse = await SetUserAsVerifiedAndGetToken();

            bool done = await _unit.CommitChangesAsync();
            return done ? Success("The account was validated" , tokenResponse)
                        : Error("An error has occurred while saving the changes" , new TokenResponse { });
        }

        private void SetEmailVerificationAsValidated(EmailVerification emailVerification) {
            emailVerification.WasValidated = true;
            _unit.EmailVerificationRepository.Update(emailVerification);
        }

        private async Task<TokenResponse> SetUserAsVerifiedAndGetToken() {
            int principalId = GetUserIdFromPrincipal();
            var user = await _unit.UserRepository.FindUserByIdAsync(principalId);

            if (user == null) {
                throw new InvalidOperationException("The user you try to verified does not exist");
            }

            user.Role = UserRoles.VerifiedUser;
            _unit.UserRepository.UpdateUser(user);

            return BuildToken(user);
        }

        public async Task<EmptyOperationResponse> SendAccountVerificationEmail(string email) { //TODO: Create a re-send method
            //string principalMail = GetMailFromPrincipal();
            if (!email.IsValidEmail()) {
                return Error("Invalid email" , new { });
            }

            var user = await _unit.UserRepository.FindUserByEmailAsync(email); //TODO: This can be better using the email as the primary key. Fix this and validate
            if (user == null)
                throw new InvalidOperationException("the user you are trying to send the verification email does not exist");

            int recoveryPin = Utilities.GetRandomPin(PIN_LENGTH);

            MailMessage msg = new MailMessage(); 
            msg.To.Add(email); 
            msg.Subject = "Account verification";   
            msg.SubjectEncoding = Encoding.UTF8;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.From = new MailAddress("eladri-@live.com", "Task Manager", Encoding.UTF8);
            msg.Body = "<h1>Task Manager</h1></br>" +
                      $"<p>Hi {email} this is your verification code: <strong>{recoveryPin}</strong> </p>"; 

            var operationResponse = await _mailService.SendEmailAsync(msg);
            if (!operationResponse.IsSuccess) {
                return operationResponse;
            }

            await SaveRecoveryPinWithoutCommit(recoveryPin, user.Id);
            bool done = await _unit.CommitChangesAsync();

            return done ? Success("Verification pin sent and save successfully" , new { })
                        : Error("An error has occurred while saving the recovery code", new { });
        }

        private async Task SaveRecoveryPinWithoutCommit(int pin, int userId) {
            int principalId = userId;

            await _unit.EmailVerificationRepository.CreateAsync(new EmailVerification() {
                Id = 0,
                RecoveryCode = pin ,
                UserId = principalId ,
                CreatedOn = DateTime.UtcNow ,
                ExpirationDate = DateTime.UtcNow.AddHours(PIN_EXPIRATION_HOURS),
                WasValidated = false
            });
        }

        private int GetUserIdFromPrincipal() {
            return Convert.ToInt32(_httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private string GetMailFromPrincipal() {
            return _httpContext.User.FindFirstValue(ClaimTypes.Email);
        }

        private TokenResponse BuildToken(ApplicationUser user) {
            if (!user.IsValid()) {
                return new TokenResponse();
            }
            //Setting up the claims
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Jti is the unique identifier of de Token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtKey").Value));  //Creating the Jwt Signature Key. JwtKey is an environment variable
            var credencials = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);   //Creating the credentials using the key and the specified algorithm
            var expirationDate = DateTime.UtcNow.AddHours(24); // Expiration time of the token

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

        public OperationResponse<Dictionary<string , object>> GetClaimsFromToken(string token) {
            TokenValidationParameters validationParameters = getValidationParamerers();
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal;
            var claimsDictionary = new Dictionary<string , object>();
            try {
                principal = tokenHandler.ValidateToken(token , validationParameters , out _);
            } catch (Exception) {

                return Error("Invalid token." , new Dictionary<string, object>());
            }

            foreach (var claim in principal.Claims) {
                claimsDictionary.Add(claim.Type , claim.Value.ToString());
            }
            return Success("Here you are" , claimsDictionary);
        }

        private TokenValidationParameters getValidationParamerers() {
            return new TokenValidationParameters {
                ValidateIssuer = true ,
                ValidateAudience = true ,
                ValidateLifetime = true ,
                ValidateIssuerSigningKey = true ,
                ValidIssuer = "yourdomain.com" ,
                ValidAudience = "yourdomain.com" ,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtKey").Value)) ,
                ClockSkew = TimeSpan.Zero
            };
        }

        
    }
}
