using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Blazor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Tests {
    [TestClass()]
    public class JwtHelperTests {
        string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJlbGFkcmktQGxpdmUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImp0aSI6ImQ5OTMwMmM0LWY3ODgtNDM0NC1iMjViLTY1OTE2ODg0OTRjZSIsImV4cCI6MTYyMDUxODEzNywiaXNzIjoieW91cmRvbWFpbi5jb20iLCJhdWQiOiJ5b3VyZG9tYWluLmNvbSJ9.iDYeJnUgkQkqAeZR3aSsapgO6p-i7aJt-m40HiRig9c";
        [TestMethod()]
        public void GetPrincipalFromTokenAsyncTest() {
            var principal = JwtHelper.GetPrincipalFromTokenAsync(token);
            Assert.IsTrue(principal.Identity.IsAuthenticated);
        }
    }
}