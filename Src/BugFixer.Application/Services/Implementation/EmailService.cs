using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Interfaces;
using System.Net.Mail;

namespace BugFixer.Application.Services.Implementation
{
    public class EmailService : IEmailService
    {
        #region Ctor

        private readonly ISiteSettingRepository _siteSettingRepository;
        public EmailService(ISiteSettingRepository siteSettingRepository)
        {
            _siteSettingRepository = siteSettingRepository;
        }
        #endregion


        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var defaultSiteEmail = await _siteSettingRepository.GetEmailDefaultSettingAsync();

                MailMessage mail = new MailMessage();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                SmtpClient SmtpServer = new SmtpClient(defaultSiteEmail.SMTP);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                mail.From = new MailAddress(defaultSiteEmail.From, defaultSiteEmail.DisplayName);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                if (defaultSiteEmail.Port != 0)
                {
                    SmtpServer.Port = defaultSiteEmail.Port;
                    SmtpServer.EnableSsl = defaultSiteEmail.EnableSSL;
                }

                SmtpServer.Credentials = new System.Net.NetworkCredential(defaultSiteEmail.From, defaultSiteEmail.Password);
                SmtpServer.Send(mail);

                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
    }
}
