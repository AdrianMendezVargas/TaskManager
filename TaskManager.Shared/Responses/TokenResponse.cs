using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared.Responses {
    public class TokenResponse {

        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }

    }
}
