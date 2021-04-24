using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Shared.Requests;

namespace TaskManager.Tests {
    [TestClass()]
    public class UserRepositoryTests {

        private UserRepository UserRepository;

        [TestInitialize]
        public void Initialize() {
            var db = DbContextHelper.GetSeedInMemoryDbContext();
            UserRepository = new UserRepository(db);
        }


        [TestMethod()]
        public async Task FindUserByCredentials_ShoulReturnTheUserWithTheMatchedCredentials() {

            var credential = new LoginRequest {
                Email = "eladri-@live.com" ,
                Password = "clave"
            };

            var user = await UserRepository.FindUserByCredentialsAsync(credential);


            Assert.IsTrue(credential.Email == user.Email);
        }

        [TestMethod()]
        public async Task FindUser_WithEmptyCredentials_ShoulReturnNull() {

            var credential = new LoginRequest {
                Email = "" ,
                Password = ""
            };

            var user = await UserRepository.FindUserByCredentialsAsync(credential);

            Assert.IsNull(user);
        }

        [TestMethod()]
        public async Task FindUser_WithEmptyPassword_ShoulReturnNull() {

            var credential = new LoginRequest {
                Email = "eladri-@live.com" ,
                Password = ""
            };

            var user = await UserRepository.FindUserByCredentialsAsync(credential);

            Assert.IsNull(user);
        }

    }
}