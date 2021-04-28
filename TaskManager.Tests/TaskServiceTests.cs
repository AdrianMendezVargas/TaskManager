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

namespace TaskManager.Tests {
    [TestClass()]
    public class TaskServiceTests {

        private TaskService TaskService;

        [TestInitialize]
        public void Initialize() {
            var db = DbContextHelper.GetSeedInMemoryDbContext();
            var unit = new EfUnitOfWork(db);
            TaskService = new TaskService(unit);
        }

        [TestMethod()]
        public async Task CreateTaskAsyncTest() {
            //Arrange
            UserTask task = new UserTask {
                Name = "Tarea 1" ,
                State = TaskState.NotStarted ,
                CreatedOn = DateTime.Now
            };

            //Action
            var result = await TaskService.CreateTaskAsync(task);

            //Asure
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod()]
        public async Task CreateTaskWithoutName_ShouldNotCreateATask() {
            UserTask task = new UserTask {
                Name = "" ,
                State = TaskState.NotStarted ,
                CreatedOn = DateTime.Now
            };

            var result = await TaskService.CreateTaskAsync(task);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod()]
        public async Task DeleteExistingTaskTest_ShouldDeleteTheTask() {
            var result = await TaskService.DeleteTaskAsync(1);
            Assert.IsTrue(result.IsSuccess);

        }

        [TestMethod()]
        public async Task DeleteNonExistingTaskTest_ShouldNotDelete() {
            var result = await TaskService.DeleteTaskAsync(7);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Record);

        }

        [TestMethod()]
        public async Task GetAllTaskAsyncTest() {
            var tasks = await TaskService.GetAllTaskAsync();
            Assert.IsTrue(tasks.Record.Any());
        }
    }
}