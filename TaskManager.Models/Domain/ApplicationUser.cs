using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Shared.Enums;

namespace TaskManager.Models.Domain {
    public class ApplicationUser : Record{

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        public string Role { get; set; } = UserRoles.NotVerifiedUser;

        public virtual List<UserTask> Tasks { get; set; }
        public virtual List<EmailVerification> EmailVerifications { get; set; }

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
