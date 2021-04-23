using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Models.Domain;

namespace TaskManager.Repository {
    public class UserRepository : IUserRepository {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db) {
            _db = db;
        }
        public async Task CreateUserAsync(ApplicationUser user) {
            await _db.Users.AddAsync(user);
        }

        public async Task<ApplicationUser> FindUserAsync(int userId) {
            return await _db.Users.FindAsync(userId);
        }

        public void RemoveUser(ApplicationUser user) {
            _db.Users.Remove(user);
        }

        public void UpdateUser(ApplicationUser user) {
            _db.Users.Update(user);
        }
    }
}
