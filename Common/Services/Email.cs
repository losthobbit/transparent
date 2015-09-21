using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    /// <summary>
    /// For sending an email via SMTP
    /// </summary>
    /// <remarks>
    /// Designed to be used as a singleton.
    /// </remarks>
    public class Email: IEmail, IDisposable
    {
        private SmtpClient smtpClient;

        private readonly IConfiguration configuration;

        public Email(IConfiguration configuration)
        {
            this.configuration = configuration;

            smtpClient = new SmtpClient
            {
                Host = configuration.GetValue("SmtpServer"),
                Credentials = new NetworkCredential(configuration.GetValue("SmtpUsername"), configuration.GetValue("SmtpPassword"))
            };
        }

        public void Send(string subject, string html, string toAddress, string fromAddress = null)
        {
            var mailMsg = new MailMessage(fromAddress ?? configuration.GetValue("DefaultFromAddress"), toAddress)
            {
                Subject = subject,
                Body = html,
                IsBodyHtml = true
            };
            smtpClient.Send(mailMsg);
        } 

        public void Dispose()
        {
            smtpClient.Dispose();
        }
    }
}
