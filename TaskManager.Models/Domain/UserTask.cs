﻿using System;
using System.ComponentModel.DataAnnotations;
using TaskManager.Models.Domain.Enums;

namespace TaskManager.Models.Domain {
    public class UserTask : Record {

        [Required]
        [StringLength(maximumLength:30, MinimumLength = 1, ErrorMessage = "Invalid task name")]
        public string Name { get; set; }
        public string State { get; set; } = TaskState.Done;

    }
}
