using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Models.Domain;
using TaskManager.Shared;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Repository {
    public class UserRepository : IUserRepository {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db) {
            _db = db;
        }
        public async Task CreateUserAsync(ApplicationUser user) {
            await _db.Users.AddAsync(user);
        }

        public async Task<ApplicationUser> FindUserByCredentialsAsync(LoginRequest credentials) {
            var passwordHash = Utilities.GetSHA256(credentials.Password);
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == credentials.Email && u.Password == passwordHash);
            return user;
        }

        public async Task<ApplicationUser> FindUserByEmailAsync(string userEmail) {
            return await _db.Users.SingleOrDefaultAsync(u => u.Email == userEmail);
        }

        public async Task<ApplicationUser> FindUserByIdAsync(int userId) {
            return await _db.Users.SingleOrDefaultAsync(u => u.Id == userId);
        }

        public void RemoveUser(ApplicationUser user) {
            _db.Users.Remove(user);
        }

        public void UpdateUser(ApplicationUser user) {
            _db.Users.Update(user);
        }
    }
}
