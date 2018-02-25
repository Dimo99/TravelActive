using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace TravelActive.Services
{
    public class EmailService 
    {
        private readonly AuthMessageSenderOptions emailOptions;

        public EmailService(IOptions<AuthMessageSenderOptions> emailOptions)
        {
            this.emailOptions = emailOptions.Value;
        }

        public Task SendEmailConfirmationAsync(string email, string token)
        {
            return this.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by inserting this token {token}");
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("travelactivebulgaria@gmail.com");
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            var smtpClient = new SmtpClient(host: emailOptions.Host,port:25)
            {
                Credentials = new NetworkCredential(emailOptions.Username,emailOptions.Password),
            };
            await smtpClient.SendMailAsync(mailMessage);
        }

        public Task SendForgotenPasswordEmailAsync(string userEmail, string message)
        {
            return this.SendEmailAsync(userEmail, "Forgoten password",
                $"To recover your password paste this token in the app \n token: {message}");
        }
    }
}