using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Repository {
    public interface IUnitOfWork {

        public ITaskRepository TaskRepository { get; }

        Task<bool> CommitChangesAsync();

    }
}
