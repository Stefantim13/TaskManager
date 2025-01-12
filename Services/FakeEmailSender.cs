using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace TaskManager.Services // Folosește namespace-ul tău
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Simulează trimiterea emailului (nu trimite efectiv nimic)
            return Task.CompletedTask;
        }
    }
}
