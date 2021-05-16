using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Repository {
    public interface IUnitOfWork {

        public ITaskRepository TaskRepository { get; }
        public IUserRepository UserRepository { get; }
        public IEmailVerificationRepository EmailVerificationRepository { get; }


        Task<bool> CommitChangesAsync();

    }
}
