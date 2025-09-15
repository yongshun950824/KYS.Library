using KYS.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace KYS.Library.Helpers
{
    public static class EmailHelper
    {
        public static void Send(SmtpClientConfig smtpClientConfig,
            string subject,
            string body,
            HashSet<string> toEmailAddresses,
            HashSet<string> ccEmailAddresses = null,
            HashSet<string> bccEmailAddresses = null,
            bool isBodyHtml = false)
        {
            SmtpClient smtpClient = (SmtpClient)smtpClientConfig;

            MailMessage message = new()
            {
                From = new MailAddress(smtpClientConfig.SenderEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            if (String.IsNullOrEmpty(subject))
                throw new ArgumentException("Must provide the Subject.");

            if (String.IsNullOrEmpty(body))
                throw new ArgumentException("Must provide the Body.");

            if (toEmailAddresses.IsNullOrEmpty())
                throw new ArgumentException("Must provide at least 1 Recipient(s).");

            if (!ccEmailAddresses.IsNullOrEmpty())
            {
                foreach (string cc in ccEmailAddresses)
                {
                    message.CC.Add(cc);
                }
            }

            if (!bccEmailAddresses.IsNullOrEmpty())
            {
                foreach (string bcc in bccEmailAddresses)
                {
                    message.Bcc.Add(bcc);
                }
            }

            smtpClient.Send(message);
        }

        public class SmtpClientConfig
        {
            public SmtpClientConfig(string host,
                int port,
                string senderEmail,
                string senderPassword,
                bool enableSSL = false,
                bool useDefaultCredentials = false)
            {
                Host = host;
                Port = port;
                SenderEmail = senderEmail;
                SenderPassword = senderPassword;
                EnableSSL = enableSSL;
                UseDefaultCredentials = useDefaultCredentials;
            }

            public string Host { get; set; }
            public int Port { get; set; }
            public string SenderEmail { get; set; }
            public string SenderPassword { get; set; }
            public bool EnableSSL { get; set; }
            public bool UseDefaultCredentials { get; set; }

            public static implicit operator SmtpClient(SmtpClientConfig smtpClientConfig)
            {
                return new SmtpClient(smtpClientConfig.SenderEmail)
                {

                    Port = smtpClientConfig.Port,
                    EnableSsl = smtpClientConfig.EnableSSL,
                    UseDefaultCredentials = smtpClientConfig.UseDefaultCredentials,
                    Credentials = !smtpClientConfig.UseDefaultCredentials
                        ? new NetworkCredential(smtpClientConfig.SenderEmail, smtpClientConfig.SenderPassword)
                        : CredentialCache.DefaultNetworkCredentials
                };
            }
        }
    }
}
