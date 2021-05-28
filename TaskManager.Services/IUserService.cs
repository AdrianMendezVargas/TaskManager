using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Services {
    public interface IUserService {
        Task<OperationResponse<TokenResponse>> RegisterUserAsync(RegisterUserRequest user);
        Task<OperationResponse<ApplicationUser>> UpdateUserAsync(ApplicationUser user);
        Task<OperationResponse<TokenResponse>> LoginUserAsync(LoginRequest credentials);
        OperationResponse<Dictionary<string, object>> GetClaimsFromToken(string token);
        Task<EmptyOperationResponse> SendAccountVerificationEmail(string email, int tryNumber);
        Task<OperationResponse<int>> ResendAccountVerificationEmail();
        Task<OperationResponse<TokenResponse>> ValidateAccountRecoveryCodeAsync(EmailVerificationRequest verificationRequest);
    }
}
