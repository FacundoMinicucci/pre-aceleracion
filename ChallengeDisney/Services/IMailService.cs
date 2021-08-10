using ChallengeDisney.Entities;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace ChallengeDisney.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(User user);
    }

    public class SendGridMailService : IMailService
    {
        private readonly ILogger<SendGridMailService> _logger;
        private readonly ISendGridClient _client;
        public SendGridMailService(ISendGridClient client, ILogger<SendGridMailService> logger)
        {
            _logger = logger; 
            _client = client;
        }
        public async Task SendEmailAsync(User user)
        {
            try
            {
                _logger.LogInformation($"Sending email for the user {user.UserName}");
                var from = new EmailAddress("disneychallengealkemy@gmail.com", "DisneyAlkemyChallenge");
                var subject = "New account created";
                var to = new EmailAddress(user.Email, user.UserName);
                var plainTextContent = "Thank you for registering in Alkemy Disney challenge!" + DateTime.Now;
                var htmlContent = "<strong>Thank you for registering in Alkemy Disney challenge!</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await _client.SendEmailAsync(msg);
            }
            catch (Exception exception)
            {
                _logger.LogError("Error sending email", exception);
                
            }          
        }
    }   
}
