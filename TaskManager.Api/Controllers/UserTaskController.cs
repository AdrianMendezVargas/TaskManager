﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models.Domain;
using TaskManager.Services;
using TaskManager.Shared.Responses;

namespace TaskManager.Api.Controllers {
    [Route("api/task")]
    [ApiController]
    public class UserTaskController : ControllerBase {

        private readonly ITaskService _taskService;
        public UserTaskController(ITaskService taskService) {
            _taskService = taskService;
        }

        [ProducesResponseType(200, Type = typeof(OperationResponse<UserTask>))]
        [ProducesResponseType(400 , Type = typeof(OperationResponse<UserTask>))]
        [HttpPost]
        public async Task<IActionResult> Post(UserTask task) {
            var result = await _taskService.CreateTaskAsync(task);
            if (result.IsSuccess) {
                return Ok(result);
            } else {
                return BadRequest(result);
            }
        }

        [ProducesResponseType(200 , Type = typeof(OperationResponse<List<UserTask>>))]
        [ProducesResponseType(400 , Type = typeof(OperationResponse<UserTask>))]
        [HttpGet]
        public async Task<IActionResult> Get() {
            var result = await _taskService.GetAllTaskAsync();
            if (result.IsSuccess) {
                return Ok(result);
            } else {
                return BadRequest(result);
            }
        }

        [ProducesResponseType(200 , Type = typeof(OperationResponse<UserTask>))]
        [ProducesResponseType(404 , Type = typeof(OperationResponse<>))]
        [HttpDelete]
        public async Task<IActionResult> Delete(int taskId) {
            var result = await _taskService.DeleteTaskAsync(taskId);
            if (result.IsSuccess) {
                return Ok(result);
            } else {
                return NotFound(result);
            }
        }

    }
}
