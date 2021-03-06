using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Models.Domain;

namespace TaskManager.Repository {
    public class TaskRepository : ITaskRepository {
        private readonly ApplicationDbContext _db;
        public TaskRepository(ApplicationDbContext db) {
            _db = db;
        }
        public async Task CreateAsync(UserTask task) {
            await _db.Tasks.AddAsync(task);
        }

        public async Task<UserTask> GetByIdAsync(int id) {
            return await _db.Tasks.FindAsync(id);
        }

        public async Task<List<UserTask>> GetUserTasks(Expression<Func<UserTask, bool>> expression) {
            return await _db.Tasks.AsNoTracking().Where(expression).ToListAsync();
        }

        public void Remove(UserTask task) {
            _db.Tasks.Remove(task);
        }

        public void Update(UserTask task) {
            _db.Tasks.Update(task);
        }
    }
}
