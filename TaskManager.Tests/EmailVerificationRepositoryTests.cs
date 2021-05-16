using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Tests {
    [TestClass()]
    public class EmailVerificationRepositoryTests {

        private IUnitOfWork Unit;

        [TestInitialize]
        public void Initialize() {
            var db = DbContextHelper.GetSeedInMemoryDbContext();
            Unit = new EfUnitOfWork(db);
        }

        [TestMethod()]
        public async Task FindLatesPinByUserId_ValidNotEpiredNotUsedPin_ShoudlReturnTheLatestEmailVerification() {
            int latesEmailVerificationId = 1;
            int userId = 1;

            var emailVerification = await Unit.EmailVerificationRepository.FindLatestByUserId(userId);

            Assert.IsTrue(latesEmailVerificationId == emailVerification.Id);
        }

        [TestMethod()]
        public async Task FindLatesPinByUserId_NotExistingUser_ShoudlReturnNull() {
            int notExistinUser = 99999;

            var emailVerification = await Unit.EmailVerificationRepository.FindLatestByUserId(notExistinUser);

            Assert.IsNull(emailVerification);
        }

    }
}