using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Data;
using TaskManager.Models.Domain;
using TaskManager.Services;
using TaskManager.Shared;

namespace TaskManager.Tests {
    public static class DbContextHelper {
        public static ApplicationDbContext GetSeedInMemoryDbContext() {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var context = new ApplicationDbContext(options);

            #region creating tasks
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
            #endregion

            #region creating users
            var users = new ApplicationUser[] {
                new ApplicationUser{
                    Id = 1,
                    Email = "eladri-@live.com",
                    Password = Utilities.GetSHA256("clave"),
                    CreatedOn = DateTime.Now
                },
                new ApplicationUser{
                    Id = 2,
                    Email = "correo@gmail.com",
                    Password = Utilities.GetSHA256("clave"),
                    CreatedOn = DateTime.Now
                }
            };
            context.Users.AddRange(users);
            #endregion

            context.SaveChanges();

            return context;
        }
    }
}
