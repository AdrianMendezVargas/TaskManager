using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared.Requests {
    public class UpdateTaskRequest {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }
}
