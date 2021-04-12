using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Repository;
using TaskManager.Shared.Responses;

namespace TaskManager.Services {
    public class TaskService : BaseService, ITaskService {

        private readonly IUnitOfWork _unit;
        public TaskService(IUnitOfWork unit) {
            _unit = unit;
        }

        public async Task<OperationResponse<UserTask>> CreateTaskAsync(UserTask task) {
            await _unit.TaskRepository.CreateAsync(task);
            await _unit.CommitChangesAsync();
            return Success<UserTask>("Task created successfully", task);
        }

        public Task<OperationResponse<UserTask>> DeleteTaskAsync(int taskId) {
            throw new NotImplementedException();
        }

        public Task<OperationResponse<List<UserTask>>> GetAllTaskAsync() {
            throw new NotImplementedException();
        }

        public Task<OperationResponse<UserTask>> GetTaskAsync(int taskId) {
            throw new NotImplementedException();
        }

        public Task<OperationResponse<UserTask>> UpdateTaskAsync(UserTask task) {
            throw new NotImplementedException();
        }
    }
}
