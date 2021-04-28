using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Services {
    public interface IUserService {
        Task<OperationResponse<TokenResponse>> RegisterUserAsync(RegisterUserRequest user);
        Task<OperationResponse<ApplicationUser>> UpdateUserAsync(ApplicationUser user);
        Task<OperationResponse<TokenResponse>> LoginUserAsync(LoginRequest credentials);
    }
}
