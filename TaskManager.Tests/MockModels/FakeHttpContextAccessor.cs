using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Tests.MockModels {
    public class FakeHttpContextAccessor : IHttpContextAccessor {
        public FakeHttpContextAccessor(HttpContext context) {
            HttpContext = context;
        }
        public HttpContext HttpContext { get ; set; }
    }
}
