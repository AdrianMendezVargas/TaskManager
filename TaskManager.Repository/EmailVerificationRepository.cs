using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;                
using TaskManager.Models.Domain;

namespace TaskManager.Repository {
    public interface IEmailVerificationRepository {
        Task CreateAsync(EmailVerification verification);
        void Remove(EmailVerification verification);
        Task<EmailVerification> FindLatestByUserId(int id);
        void Update(EmailVerification verification);

    }
    public class EmailVerificationRepository : IEmailVerificationRepository {
        private readonly ApplicationDbContext _db;

        public EmailVerificationRepository(ApplicationDbContext db) {
            _db = db;
        }

        public async Task CreateAsync(EmailVerification verification) {
            await _db.EmailVerifications.AddAsync(verification);
        }

        public async Task<EmailVerification> FindLatestByUserId(int userId) {
            var allUserEmailVerifications = await _db.EmailVerifications
              .Where(e => e.UserId == userId)
              .ToListAsync();

            if (allUserEmailVerifications == null || !allUserEmailVerifications.Any()) {
                return null;
            } else {
                return allUserEmailVerifications.OrderByDescending(e => e.CreatedOn).First();
            }
        }

        public void Update(EmailVerification verification) {
            _db.EmailVerifications.Update(verification);
        }

        public void Remove(EmailVerification verification) {
            _db.EmailVerifications.Remove(verification);
        }
    }
}
