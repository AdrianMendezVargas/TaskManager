using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Models.Domain;
using TaskManager.Services;

namespace TaskManager.Tests {
    public static class DbContextHelper {
        public static ApplicationDbContext GetSeedDbContext() {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var context = new ApplicationDbContext(options);

            var userTask = new UserTask[] {
                new UserTask {
                    Id = 1,
                    Name = "Task 1",
                    State = Models.Domain.Enums.TaskState.NotStarted,
                    CreatedOn = DateTime.Now
                },
                new UserTask {
                    Id = 2,
                    Name = "Task 2",
                    State = Models.Domain.Enums.TaskState.InProgress,
                    CreatedOn = DateTime.Now
                },
                new UserTask {
                    Id = 3,
                    Name = "Task 3",
                    State = Models.Domain.Enums.TaskState.Done,
                    CreatedOn = DateTime.Now
                }
            };
            context.Tasks.AddRange(userTask);
            context.SaveChanges();

            return context;
        }
    }
}
