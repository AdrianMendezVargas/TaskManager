using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Shared.Options {
    public class MailConfigOptions {

        public string Sender { get; set; }
        public int Port { get; set; }
        public NetworkCredential Credentials { get; set; }
        public bool EnableSSL { get; set; }

    }
}
