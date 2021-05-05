using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Models.Mappers;
using TaskManager.Repository;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Services {
    public class TaskService : BaseService, ITaskService {

        private readonly IUnitOfWork _unit;
        private readonly HttpContext _httpContext;
        public TaskService(IUnitOfWork unit, IHttpContextAccessor httpContextAccessor) {
            _unit = unit;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<OperationResponse<UserTask>> CreateTaskAsync(CreateTaskRequest taskRequest) {
            int principalId = GetUserIdFromPrincipal();
            var task = taskRequest.ToUserTask();

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
            int principalId = GetUserIdFromPrincipal();
            var task = await _unit.TaskRepository.GetByIdAsync(taskId);
            if (task != null) {

                if (task.UserId != principalId) {
                    return Error("This is not your task" , new UserTask());
                }

                _unit.TaskRepository.Remove(task);
                var done = await _unit.CommitChangesAsync();

                return done ? Success("Task was removed successfully" , task)
                            : Error("Could not remove the task" , task);
            } else {
                return Error("This task does not exist." , task);
            }
        }

        public async Task<OperationResponse<List<UserTask>>> GetAllTaskAsync() {
            int principalId = GetUserIdFromPrincipal();

            var tasks = await _unit.TaskRepository.GetUserTasks(x => true && x.UserId == principalId);
            return Success<List<UserTask>>("Here you are" , tasks);
        }

        public async Task<OperationResponse<UserTask>> GetTaskByIdAsync(int taskId) {
            int principalId = GetUserIdFromPrincipal();

            var task = await _unit.TaskRepository.GetByIdAsync(taskId);

            if (task == null) {
                return Error("This task does not exist" , new UserTask());
            }

            return task.UserId == principalId
                ? Success("Here you are" , task)
                : Error("Not authorized to view this task" , new UserTask());
        }

        private int GetUserIdFromPrincipal() {
            return Convert.ToInt32(_httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public async Task<OperationResponse<UserTask>> UpdateTaskAsync(UpdateTaskRequest request) {
            int pricipalId = GetUserIdFromPrincipal();
            var task = await _unit.TaskRepository.GetByIdAsync(request.Id);

            if (task == null) {
                return Error("This task does not exist" , new UserTask());
            }

            if (task.UserId != pricipalId) {
                return Error("You can not update this task", new UserTask());
            }

            task.Name = request.Name;
            task.State = request.State;

            if (!task.IsValid()) {
                return Error("Invalid task data" , new UserTask());
            }

            _unit.TaskRepository.Update(task);
            bool done = await _unit.CommitChangesAsync();

            return done ? Success("Task was updated successfully" , task)
                        : Error("Could not update the task" , task);

        }
    }
}
