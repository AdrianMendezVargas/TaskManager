using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Tests {
    [TestClass()]
    public class StringExtensionsTests {
        [TestMethod()]
        public void IsValidEmail_ValidEmail_ShouldReturnTrue() {
            string email = "eladri-@live.com";
            Assert.IsTrue(email.IsValidEmail());
        }

        [TestMethod()]
        public void IsValidEmail_ValidEmail_ShouldReturnTrue2() {
            string email = "velociraptor088@gmail.com";
            Assert.IsTrue(email.IsValidEmail());
        }

        [TestMethod()]
        public void IsValidEmail_ValidEmail_ShouldReturnTrue3() {
            string email = "velociraptor088@gmail.es";
            Assert.IsTrue(email.IsValidEmail());
        }

        [TestMethod()]
        public void IsValidEmail_InValidEmail_ShouldReturnFalse() {
            string email = "velociraptor088gmail.es";
            Assert.IsFalse(email.IsValidEmail());
        }

        [TestMethod()]
        public void IsValidEmail_InValidEmail_ShouldReturnFalse2() {
            string email = "velociraptor088@gmailes";
            Assert.IsFalse(email.IsValidEmail());
        }

        [TestMethod()]
        public void IsValidEmail_InValidEmail_ShouldReturnFalse3() {
            string email = "email";
            Assert.IsFalse(email.IsValidEmail());
        }

    }
}