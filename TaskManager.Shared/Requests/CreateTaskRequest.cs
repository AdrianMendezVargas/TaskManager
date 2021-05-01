using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared.Requests {
    public class CreateTaskRequest {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
    }
}
