using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        public async Task<OperationResponse<UserTask>> CreateTaskAsync(ClaimsPrincipal claimsPrincipal, UserTask task) {
            int principalId = Convert.ToInt32(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));


            if (string.IsNullOrWhiteSpace(task.Name)) {
                return Error("The task must have a name" , task);
            }

            #region setting default values
            task.Id = 0;
            task.CreatedOn = DateTime.UtcNow;
            task.UserId = principalId;
            #endregion

            await _unit.TaskRepository.CreateAsync(task);
            var done = await _unit.CommitChangesAsync();

            return done ? Success("Task created successfully" , task)
                        : Error("Could not created the task" , task);
        }

        public async Task<OperationResponse<UserTask>> DeleteTaskAsync(int taskId) {

            //TODO: Validate the user owner before deleting
            var task = await _unit.TaskRepository.GetByIdAsync(taskId);
            if (task != null) {
                _unit.TaskRepository.Remove(task);
                var done = await _unit.CommitChangesAsync();

                return done ? Success("Task was removed successfully" , task)
                            : Error("Could not remove the task" , task);
            } else {
                return Error("This task does not exist." , task);
            }
        }

        public async Task<OperationResponse<List<UserTask>>> GetAllTaskAsync(ClaimsPrincipal claimsPrincipal) {
            int principalId = Convert.ToInt32(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

            var tasks = await _unit.TaskRepository.GetUserTasks(x => true && x.UserId == principalId);
            return Success<List<UserTask>>("Here you are" , tasks);
        }

        public async Task<OperationResponse<UserTask>> GetTaskByIdAsync(int taskId , ClaimsPrincipal claimsPrincipal) {
            var task = await _unit.TaskRepository.GetByIdAsync(taskId);
            int userId = Convert.ToInt32(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

            return task.UserId == userId 
                ? Success("Here you are" , task) 
                : Error("Not authorized to view this task" , new UserTask());
        }

        public Task<OperationResponse<UserTask>> UpdateTaskAsync(UserTask task) {
            throw new NotImplementedException();
        }
    }
}
