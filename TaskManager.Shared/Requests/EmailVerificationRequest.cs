using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared.Requests {
    public class EmailVerificationRequest {

        [Required]
        public int Code { get; set; }

    }
}
