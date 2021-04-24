﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared.Requests {
    public class LoginRequest {
        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsValid() {
            bool valid = true;
            if (string.IsNullOrWhiteSpace(Email)) {
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(Password)) {
                valid = false;
            }
            return valid;
        }

    }
}
