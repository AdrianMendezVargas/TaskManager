using System;
using TaskManager.Models.Domain.Enums;

namespace TaskManager.Models.Domain {
    public class Task : Record {

        public string Name { get; set; }
        public TaskState State { get; set; }

    }
}
