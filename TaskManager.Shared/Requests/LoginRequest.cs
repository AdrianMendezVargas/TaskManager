using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Shared.Extensions;

namespace TaskManager.Shared.Requests {
    public class LoginRequest {
        [Required]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public bool IsValid() {
            bool valid = true;
            if (!Email.IsValidEmail()) {
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(Password)) {
                valid = false;
            }
            return valid;
        }

    }
}
