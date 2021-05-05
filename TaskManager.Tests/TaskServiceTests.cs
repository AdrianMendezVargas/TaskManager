using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Repository;
using TaskManager.Models.Domain;
using TaskManager.Shared.Enums;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TaskManager.Shared.Requests;
using Microsoft.AspNetCore.Http;
using Moq;

namespace TaskManager.Tests {
    [TestClass()]
    public class TaskServiceTests {

        private const string userToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJlbGFkcmktQGxpdmUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImp0aSI6ImZhZDYzYmFlLTU4ZTItNDk2Yy1iM2U0LTczZGQxM2MxZWE0MiIsImlzcyI6InlvdXJkb21haW4uY29tIiwiYXVkIjoieW91cmRvbWFpbi5jb20ifQ.JWFhxzQG6jqVkNPN6dYjBDtzPE9UZFMsG6LvWViCwFQ";
        private const string secretKey = "ClaveSuperSecretAZNM82HDY7Y1PSKCMX9JD712JSGH";
        private TaskService TaskService;
        private ClaimsPrincipal ClaimsPrincipal;
        private IUnitOfWork Unit;

        [TestInitialize]
        public void Initialize() {
            var db = DbContextHelper.GetSeedInMemoryDbContext();
            Unit = new EfUnitOfWork(db);

            ClaimsPrincipal = GetClaimsPrincipalFromToken(userToken);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.User).Returns(ClaimsPrincipal);

            var httpContextAccesor = new Mock<IHttpContextAccessor>();
            httpContextAccesor.Setup(h => h.HttpContext).Returns(httpContext.Object);

            TaskService = new TaskService(Unit, httpContextAccesor.Object);
        }

        [TestMethod()]
        public async Task CreateTaskAsyncTest() {
            //Arrange
            int principalId = Convert.ToInt32(ClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
            var task = new CreateTaskRequest {
                Name = "Tarea" ,
            };

            //Action
            var result = await TaskService.CreateTaskAsync(task);

            //Asure
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Record.UserId == principalId);
        }

        [TestMethod()]
        public async Task CreateTaskWithoutName_ShouldNotCreateATask() {
            var task = new CreateTaskRequest {
                Name = "" ,
            };

            var result = await TaskService.CreateTaskAsync(task);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod()]
        public async Task DeleteExistingTaskTest_ShouldDeleteTheTask() {
            var result = await TaskService.DeleteTaskAsync(2);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod()]
        public async Task DeleteNotOwnedTask_ShouldNotDeleteTheTask() {
            int notOwnedTaskId = 1;

            var result = await TaskService.DeleteTaskAsync(notOwnedTaskId);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod()]
        public async Task DeleteNonExistingTaskTest_ShouldNotDelete() {
            var result = await TaskService.DeleteTaskAsync(99);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Record);

        }

        [TestMethod()]
        public async Task GetAllTaskAsyncTest() {
            int principalId = Convert.ToInt32(ClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await TaskService.GetAllTaskAsync();

            bool pass = true;
            result.Record.TrueForAll(t => t.UserId == principalId);

            Assert.IsTrue(pass);
        }

        [TestMethod()]
        public async Task GetTaskByIdAsync_IsTheOwner_ShouldReturnTheRequestedTask() {
            int seededTaskId = 2;
            int principalId = Convert.ToInt32(ClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await TaskService.GetTaskByIdAsync(seededTaskId);

            Assert.IsTrue(seededTaskId == result.Record.Id);
            Assert.IsTrue(result.Record.UserId == principalId);
        }

        [TestMethod()]
        public async Task GetTaskById_NotTheOwner_ShouldNotReturnTheRequestedTask() {
            int seededTaskId = 1;
            string principalEmail = ClaimsPrincipal.FindFirstValue(ClaimTypes.Email); //eladri-@live.com
            var user = await Unit.UserRepository.FindUserByEmailAsync(principalEmail);

            var result = await TaskService.GetTaskByIdAsync(seededTaskId);

            Assert.IsTrue(!result.IsSuccess);
            Assert.IsTrue(result.Record.Id == 0);
        }

        [TestMethod()]
        public async Task GetTaskById_NotExistingTask_ShouldNotReturnTheRequestedTask() {
            int notExistingTaskId = 99;
            string principalEmail = ClaimsPrincipal.FindFirstValue(ClaimTypes.Email); //eladri-@live.com

            var result = await TaskService.GetTaskByIdAsync(notExistingTaskId);

            Assert.IsTrue(!result.IsSuccess);
            Assert.IsTrue(result.Record.Id == 0);
        }

        [TestMethod()]
        public async Task UpdateTask_ValidOwnedTask_ShouldUpdateAndReturnTheTask() {
            int taskId = 2;

            var updateRequest = new UpdateTaskRequest {
                Id = taskId ,
                Name = "Nuevo nombre",
                State = TaskState.Done
            };

            var result = await TaskService.UpdateTaskAsync(updateRequest);

            Assert.IsTrue(result.Record.Name == updateRequest.Name);
            Assert.IsTrue(result.Record.State == updateRequest.State);
            Assert.IsTrue(result.Record.Id == updateRequest.Id);
        }

        [TestMethod()]
        public async Task UpdateTask_InvalidNameOwnedTask_ShouldNotUpdateTheTask() {
            int taskId = 2;

            var updateRequest = new UpdateTaskRequest {
                Id = taskId ,
                Name = "" ,
                State = TaskState.Done
            };

            var result = await TaskService.UpdateTaskAsync(updateRequest);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Record.Name != updateRequest.Name);
            Assert.IsTrue(result.Record.State != updateRequest.State);
            Assert.IsTrue(result.Record.Id != updateRequest.Id);
        }

        [TestMethod()]
        public async Task UpdateTask_InvalidStateOwnedTask_ShouldNotUpdateTheTask() {
            int taskId = 2;

            var updateRequest = new UpdateTaskRequest {
                Id = taskId ,
                Name = "tarea" ,
                State = "invalid state"
            };

            var result = await TaskService.UpdateTaskAsync(updateRequest);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Record.Name != updateRequest.Name);
            Assert.IsTrue(result.Record.State != updateRequest.State);
            Assert.IsTrue(result.Record.Id != updateRequest.Id);
        }

        [TestMethod()]
        public async Task UpdateTask_ValidNotOwnedTask_ShouldNotUpdateTheTask() {
            int taskId = 1;

            var updateRequest = new UpdateTaskRequest {
                Id = taskId ,
                Name = "Nuevo nombre" ,
                State = TaskState.Done
            };

            var result = await TaskService.UpdateTaskAsync(updateRequest);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Record.Name != updateRequest.Name);
            Assert.IsTrue(result.Record.State != updateRequest.State);
            Assert.IsTrue(result.Record.Id != updateRequest.Id);
        }

        [TestMethod()]
        public async Task UpdateTask_NotExistingTask_ShouldNotUpdateTheTask() {
            int taskId = 99;

            var updateRequest = new UpdateTaskRequest {
                Id = taskId ,
                Name = "Nuevo nombre" ,
                State = TaskState.Done
            };

            var result = await TaskService.UpdateTaskAsync(updateRequest);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.Record.Name != updateRequest.Name);
            Assert.IsTrue(result.Record.State != updateRequest.State);
            Assert.IsTrue(result.Record.Id != updateRequest.Id);
        }

        public ClaimsPrincipal GetClaimsPrincipalFromToken(string token) {
            SecurityToken securityToken;
            TokenValidationParameters validationParameters = getValidationParamerers();
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token , validationParameters , out securityToken);
            return principal;
        }

        private static TokenValidationParameters getValidationParamerers() {
            return new TokenValidationParameters {
                ValidateIssuer = true ,
                ValidateAudience = true ,
                ValidateLifetime = false ,
                ValidateIssuerSigningKey = true ,
                ValidIssuer = "yourdomain.com" ,
                ValidAudience = "yourdomain.com" ,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) ,
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}