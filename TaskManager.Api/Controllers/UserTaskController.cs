using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Models.Mappers;
using TaskManager.Services;
using TaskManager.Shared.Enums;
using TaskManager.Shared.Models;
using TaskManager.Shared.Requests;
using TaskManager.Shared.Responses;

namespace TaskManager.Api.Controllers {
    [Route("api/task")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.VerifiedUser + ", " + UserRoles.Admin)] //5. Require to be authorized to use this controller
    public class UserTaskController : ControllerBase {

        private readonly ITaskService _taskService;
        public UserTaskController(ITaskService taskService) {
            _taskService = taskService;
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<UserTaskDetails>))]
        [ProducesResponseType(400 , Type = typeof(OperationResponse<object>))]
        [HttpPost]
        public async Task<IActionResult> Post(CreateTaskRequest taskRequest) {
            var result = await _taskService.CreateTaskAsync(taskRequest);
            if (result.IsSuccess) {
                return Ok(new { 
                    Message = result.Message,
                    IsSuccess = result.IsSuccess,
                    Record = result.Record.ToUserTaskDetails()
                });
            } else {
                return BadRequest(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                });
            }
        }

        [ProducesResponseType(200 , Type = typeof(OperationResponse<List<UserTaskDetails>>))]
        [ProducesResponseType(400 , Type = typeof(OperationResponse<object>))]
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var result = await _taskService.GetAllTaskAsync();
            if (result.IsSuccess) {
                return Ok(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                    Record = result.Record.ToUserTaskDetailsList()
                });
            } else {
                return BadRequest(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                });
            }
        }

        [ProducesResponseType(200 , Type = typeof(OperationResponse<UserTaskDetails>))]
        [ProducesResponseType(404 , Type = typeof(OperationResponse<object>))]
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> Delete(int taskId) {
            var result = await _taskService.DeleteTaskAsync(taskId);
            if (result.IsSuccess) {
                return Ok(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                    Record = result.Record.ToUserTaskDetails()
                });
            } else {
                return NotFound(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                });
            }
        }

        [ProducesResponseType(200 , Type = typeof(OperationResponse<UserTaskDetails>))]
        [ProducesResponseType(404 , Type = typeof(OperationResponse<object>))]
        [HttpGet("{taskId}")]
        public async Task<IActionResult> Get(int taskId) {
            var result = await _taskService.GetTaskByIdAsync(taskId);
            if (result.IsSuccess) {
                return Ok(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                    Record = result.Record.ToUserTaskDetails()
                });
            } else {
                return NotFound(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                });
            }
        }

        [ProducesResponseType(200 , Type = typeof(OperationResponse<UserTaskDetails>))]
        [ProducesResponseType(404 , Type = typeof(OperationResponse<object>))]
        [HttpPut]
        public async Task<IActionResult> Put(UpdateTaskRequest request) {
            var result = await _taskService.UpdateTaskAsync(request);
            if (result.IsSuccess) {
                return Ok(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                    Record = result.Record.ToUserTaskDetails()
                });
            } else {
                return NotFound(new {
                    Message = result.Message ,
                    IsSuccess = result.IsSuccess ,
                });
            }
        }

    }
}
