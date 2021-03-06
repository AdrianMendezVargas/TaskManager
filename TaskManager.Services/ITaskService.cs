using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Shared.Models;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Services {
    public interface ITaskService {

        Task<OperationResponse<UserTask>> CreateTaskAsync(CreateTaskRequest task);
        Task<OperationResponse<UserTask>> DeleteTaskAsync(int taskId);
        Task<OperationResponse<UserTask>> GetTaskByIdAsync(int taskId);
        Task<OperationResponse<UserTask>> UpdateTaskAsync(UpdateTaskRequest task);
        Task<OperationResponse<List<UserTask>>> GetAllTaskAsync();

    }
}
