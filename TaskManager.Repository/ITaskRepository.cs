using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Domain;

namespace TaskManager.Repository {
    public interface ITaskRepository {
        Task CreateAsync(UserTask task);
        Task<UserTask> GetByIdAsync(int id);
        void Remove(UserTask task);
        void Update(UserTask task);
        Task<List<UserTask>> GetUserTasks(Expression<Func<UserTask , bool>> expression);

    }
}
