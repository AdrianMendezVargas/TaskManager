using Microsoft.EntityFrameworkCore;
using System;
using TaskManager.Models.Data;
using TaskManager.Models.Domain;
using TaskManager.Shared;
using TaskManager.Shared.Enums;

namespace TaskManager.Tests {
    public static class DbContextHelper {
        public static ApplicationDbContext GetSeedInMemoryDbContext() {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            var context = new ApplicationDbContext(options);

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
            #endregion

            #region creating tasks
            var userTask = new UserTask[] {
                new UserTask {
                    Id = 1,
                    Name = "Task 1",
                    State = TaskState.NotStarted,
                    CreatedOn = DateTime.Now,
                    UserId = 2
                },
                new UserTask {
                    Id = 2,
                    Name = "Task 2",
                    State = TaskState.InProgress,
                    CreatedOn = DateTime.Now,
                    UserId = 1
                },
                new UserTask {
                    Id = 3,
                    Name = "Task 2",
                    State = TaskState.InProgress,
                    CreatedOn = DateTime.Now,
                    UserId = 1
                }
            };
            #endregion


            context.Users.AddRange(users);
            context.Tasks.AddRange(userTask);

            context.SaveChanges();

            return context;
        }
    }
}
