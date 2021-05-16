using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Shared.Options;
using TaskManager.Shared.Responses;

namespace TaskManager.Shared.Services {
    public class MailService : BaseSharedService, IMailService {
        private readonly SmtpClient _mailClient;

        public MailService(SmtpClient mailClient) {
            _mailClient = mailClient;
        }

        public async Task<EmptyOperationResponse> SendEmailAsync(MailMessage mail) {
            try {
               await _mailClient.SendMailAsync(mail);
            } catch (Exception e) {

                return Error($"Ah error has occurred while sending this email. \nError: {e.Message}");
            }
            return Success($"Email successfully sent.");
        }
    }
}
