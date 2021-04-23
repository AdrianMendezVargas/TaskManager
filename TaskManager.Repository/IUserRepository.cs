using System.Threading.Tasks;
using TaskManager.Models.Domain;

namespace TaskManager.Repository {
    public interface IUserRepository {

        Task CreateUserAsync(ApplicationUser user);
        void RemoveUser(ApplicationUser user);
        void UpdateUser(ApplicationUser user);
        Task<ApplicationUser> FindUserAsync(int userId);

    }
}
