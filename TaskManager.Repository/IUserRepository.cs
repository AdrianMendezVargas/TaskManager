using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Repository {
    public interface IUserRepository {

        Task CreateUserAsync(ApplicationUser user);
        void RemoveUser(ApplicationUser user);
        void UpdateUser(ApplicationUser user);
        Task<ApplicationUser> FindUserByIdAsync(int userId);
        Task<ApplicationUser> FindUserByCredentialsAsync(LoginRequest credentials);
        Task<ApplicationUser> FindUserByEmailAsync(string userEmail);
    }
}
