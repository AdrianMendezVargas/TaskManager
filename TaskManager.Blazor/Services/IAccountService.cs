using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Blazor.Services {
    public interface IAccountService {
        Task<OperationResponse<TokenResponse>> LoginAsync(LoginRequest login);
        Task LogoutAsync();
        Task<OperationResponse<TokenResponse>> SignUpAsync(RegisterUserRequest request);
        Task<OperationResponse<int>> ResendEmailVerificationAsync();
        Task<EmptyOperationResponse> ValidateAccountAsync(EmailVerificationRequest verificationRequest);
    }

}
