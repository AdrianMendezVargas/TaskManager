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
                    CreatedOn = DateTime.UtcNow
                },
                new ApplicationUser{
                    Id = 2,
                    Email = "correo@gmail.com",
                    Password = Utilities.GetSHA256("clave"),
                    CreatedOn = DateTime.UtcNow
                }
            };
            #endregion

            #region creating tasks
            var userTask = new UserTask[] {
                new UserTask {
                    Id = 1,
                    Name = "Task 1",
                    State = TaskState.NotStarted,
                    CreatedOn = DateTime.UtcNow,
                    UserId = 2
                },
                new UserTask {
                    Id = 2,
                    Name = "Task 2",
                    State = TaskState.InProgress,
                    CreatedOn = DateTime.UtcNow,
                    UserId = 1
                },
                new UserTask {
                    Id = 3,
                    Name = "Task 2",
                    State = TaskState.InProgress,
                    CreatedOn = DateTime.UtcNow,
                    UserId = 1
                }
            };
            #endregion

            #region creating EmailVerificaions

            var emailVerifications = new EmailVerification[] {

                //Valid - not expired - eladri-@live.com  - User Latest
                new EmailVerification {
                    Id = 1,
                    RecoveryCode = "11111",
                    WasValidated = false,
                    CreatedOn = DateTime.UtcNow.AddMinutes(-3),
                    ExpirationDate = DateTime.UtcNow.AddHours(2).AddMinutes(-3),
                    UserId = 1,
                    TryNumber = 0
                },
                //Valid - not expired - eladri-@live.com but not the latest - same recovery code
                new EmailVerification {
                    Id = 2,
                    RecoveryCode = "22222",
                    WasValidated = false,
                    CreatedOn = DateTime.UtcNow.AddHours(-10),
                    ExpirationDate = DateTime.UtcNow.AddHours(-8),
                    UserId = 1,
                    TryNumber = 0
                },
                //Valid - not expired - eladri-@live.com - already used
                new EmailVerification {
                    Id = 3,
                    RecoveryCode = "33333",
                    WasValidated = true,
                    CreatedOn = DateTime.UtcNow.AddMinutes(-5),
                    ExpirationDate = DateTime.UtcNow.AddHours(2).AddMinutes(-5),
                    UserId = 1,
                    TryNumber = 0
                },
                //Valid - not expired - eladri-@live.com  - But no Latest
                new EmailVerification {
                    Id = 4,
                    RecoveryCode = "44444",
                    WasValidated = false,
                    CreatedOn = DateTime.UtcNow.AddMinutes(-7),
                    ExpirationDate = DateTime.UtcNow.AddHours(2).AddMinutes(-7),
                    UserId = 1,
                    TryNumber = 0
                },
                //Valid - not expired - correo@gmail.com  - Table Latest
                new EmailVerification {
                    Id = 5,
                    RecoveryCode = "55555",
                    WasValidated = false,
                    CreatedOn = DateTime.UtcNow.AddMinutes(-10),
                    ExpirationDate = DateTime.UtcNow.AddHours(2).AddMinutes(-10),
                    UserId = 2,
                    TryNumber = 0
                }
            };

            #endregion


            context.Users.AddRange(users);
            context.Tasks.AddRange(userTask);
            context.EmailVerifications.AddRange(emailVerifications);

            context.SaveChanges();

            return context;
        }
    }
}
