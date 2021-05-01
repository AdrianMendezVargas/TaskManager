using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Shared.Enums;

namespace TaskManager.Models.Domain {
    public class UserTask : Record {

        [Required]
        [StringLength(maximumLength:30, MinimumLength = 1, ErrorMessage = "Invalid task name")]
        public string Name { get; set; }
        public string State { get; set; } = TaskState.NotStarted;

        public virtual ApplicationUser User { get; set; }
        public int UserId { get; set; }

        public bool IsValid() {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(Name)) {
                valid = false;
            }

            if (State != TaskState.NotStarted && State != TaskState.InProgress && State != TaskState.Done) {
                valid = false;
            }

            return valid;
        }
    }
}
