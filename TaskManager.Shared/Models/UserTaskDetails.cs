using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared.Models {
    public class UserTaskDetails {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
