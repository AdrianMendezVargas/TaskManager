﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Shared.Responses;

namespace TaskManager.Services {
    public interface ITaskService {

        Task<OperationResponse<UserTask>> CreateTaskAsync(UserTask task);
        Task<OperationResponse<UserTask>> DeleteTaskAsync(int taskId);
        Task<OperationResponse<UserTask>> GetTaskAsync(int taskId);
        Task<OperationResponse<UserTask>> UpdateTaskAsync(UserTask task);
        Task<OperationResponse<List<UserTask>>> GetAllTaskAsync();

    }
}
