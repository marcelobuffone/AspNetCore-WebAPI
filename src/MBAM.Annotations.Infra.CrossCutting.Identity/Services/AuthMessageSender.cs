using System.Threading.Tasks;

namespace MBAM.Annotations.Infra.CrossCutting.Identity.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //implement ~~
            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            //implement ~~
            return Task.FromResult(0);
        }
    }
}
