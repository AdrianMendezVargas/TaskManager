using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Domain {
    public class EmailVerification : Record {

        public int RecoveryCode { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool WasValidated { get; set; }
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public bool IsExpired() {
            return DateTime.UtcNow >= ExpirationDate;
        }

    }
}
