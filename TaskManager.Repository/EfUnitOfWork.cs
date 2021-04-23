using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;

namespace TaskManager.Repository {
    public class EfUnitOfWork : IUnitOfWork {

        private readonly ApplicationDbContext _db;
        public EfUnitOfWork(ApplicationDbContext db) {
            _db = db;
        }

        private ITaskRepository _taskRepository;
        private IUserRepository _userRepository;

        public ITaskRepository TaskRepository { 
            get {
                return _taskRepository ??= new TaskRepository(_db);
            } 
        }

        public IUserRepository UserRepository { 
            get {
                return _userRepository ??= new UserRepository(_db);
            }
        }

        public async Task<bool> CommitChangesAsync() {
            return (await _db.SaveChangesAsync()) > 0;
        }
    }
}
