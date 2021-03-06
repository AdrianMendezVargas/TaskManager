using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManager.Blazor.AppState;
using TaskManager.Shared.Models;
using TaskManager.Shared.Responses;

namespace TaskManager.Blazor.Services {
    public interface ITaskService {
        Task<OperationResponse<List<UserTaskDetails>>> GetUserTasks();
        Task<OperationResponse<UserTaskDetails>> CreateTaskAsync(UserTaskDetails taskDetails);

    }
    public class TaskService : BaseService, ITaskService {
        private readonly HttpClient _httpClient;
        private readonly Appstate _appState;

        public TaskService(HttpClient httpClient, Appstate appState) {
            _httpClient = httpClient;
            _appState = appState;
        }

        public async Task<OperationResponse<UserTaskDetails>> CreateTaskAsync(UserTaskDetails taskDetails) {
            var principal = await GetPrincipal();

            var response = await _httpClient.PostAsJsonAsync("https://localhost:44386/api/task", taskDetails);
            if (!response.IsSuccessStatusCode) {
                return Error("An error occurred while creating the tasks" , new UserTaskDetails());
            }

            var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<UserTaskDetails>>();
            if (!operationResponse.IsSuccess) {
                return Error("An error occurred while reading the tasks" , new UserTaskDetails());
            }

            return operationResponse;
        }

        public async Task<OperationResponse<List<UserTaskDetails>>> GetUserTasks() {
            var principal = await GetPrincipal();

            var response = await _httpClient.GetAsync("https://localhost:44386/api/task");
            if (!response.IsSuccessStatusCode) {
                return Error("An error occurred while getting your tasks" , new List<UserTaskDetails>());   
            }

            var operationResponse = await response.Content.ReadFromJsonAsync<OperationResponse<List<UserTaskDetails>>>();
            if (!operationResponse.IsSuccess) {
                return Error("An error occurred while reading your tasks" , new List<UserTaskDetails>());
            }

            return operationResponse;
        }

        private async Task<ClaimsPrincipal> GetPrincipal() {
            return await _appState.PrincipalAsync;
        }
    }
}
