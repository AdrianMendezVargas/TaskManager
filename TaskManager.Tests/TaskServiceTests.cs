using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Repository;
using TaskManager.Models.Domain;
using TaskManager.Models.Domain.Enums;

namespace TaskManager.Tests {
    [TestClass()]
    public class TaskServiceTests {

        private TaskService TaskService;

        [TestInitialize]
        public void Initialize() {
            var db = ApplicacionDbInitializer.GetDbContext();
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
    }
}