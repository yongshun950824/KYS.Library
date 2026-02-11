using KYS.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for sending email.
    /// <br /><br />
    /// TO-DO: <br />
    /// <list type="number">
    ///     <item>
    ///         <description>
    ///             Migrate to service for DI supported. Support different email providers such as SMTP, SendGrid and etc.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Implement unit tests.
    ///         </description>
    ///     </item>
    /// </list>
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// Send email with SMTP client.
        /// </summary>
        /// <param name="smtpClientConfig">The configuration for SMTP client.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="body">The email content.</param>
        /// <param name="toEmailAddresses">The email recipient(s).</param>
        /// <param name="ccEmailAddresses">The CC email recipient(s).</param>
        /// <param name="bccEmailAddresses">The BCC email recipient(s).</param>
        /// <param name="isBodyHtml">The <see cref="bool" /> value indicates the email <c>body</c> is a HTML.</param>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Represents the configuration settings required to initialize and send emails using an SMTP client.
        /// </summary>
        /// <remarks>
        /// This class encapsulates SMTP configuration options such as host, port, credentials, and SSL settings.
        /// It also provides an implicit conversion to <see cref="System.Net.Mail.SmtpClient"/> for convenient use.
        ///
        /// Example usage:
        /// <code>
        /// var smtpConfig = new SmtpClientConfig(
        ///     host: "smtp.gmail.com",
        ///     port: 587,
        ///     senderEmail: "example@gmail.com",
        ///     senderPassword: "password",
        ///     enableSSL: true);
        ///
        /// using var smtpClient = (SmtpClient)smtpConfig;
        /// smtpClient.Send("example@gmail.com", "recipient@example.com", "Subject", "Body");
        /// </code>
        /// </remarks>
        public class SmtpClientConfig
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SmtpClientConfig"/> class
            /// with the specified SMTP server settings and credentials.            
            /// </summary>
            /// <param name="host">The host URL of the SMTP server.</param>
            /// <param name="port">The port number used for the SMTP connection.</param>
            /// <param name="senderEmail">The email address of the sender.</param>
            /// <param name="senderPassword">The password associated with the sender’s email account.</param>
            /// <param name="enableSSL">Optional. If <see langword="true"/>, enables SSL encryption for the SMTP connection. Default is <see langword="false"/>.</param>
            /// <param name="useDefaultCredentials">Optional. If <see langword="true"/>, uses the system’s default credentials instead of the provided credentials. Default is <see langword="false"/>.</param>
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

            /// <summary>
            /// Gets or sets a <see cref="string" /> value for the host URL for the SMTP server.
            /// </summary>
            public string Host { get; set; }
            /// <summary>
            /// Gets or sets a <c>int</c> value for the port number for the the SMTP server.
            /// </summary>
            public int Port { get; set; }
            /// <summary>
            /// Gets or sets a <see cref="string" /> value for the sender's email address.
            /// </summary>
            public string SenderEmail { get; set; }
            /// <summary>
            /// Gets or sets a <see cref="string" /> value for the sender's email password.
            /// </summary>
            public string SenderPassword { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether SSL encryption should be enabled for the SMTP connection.
            /// </summary>
            public bool EnableSSL { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether to use the system’s default credentials when authenticating with the SMTP server.
            /// </summary>
            public bool UseDefaultCredentials { get; set; }

            /// <summary>
            /// Implicitly converts an instance of <see cref="SmtpClientConfig"/> to a configured instance of <see cref="SmtpClient"/>.
            /// </summary>
            /// <param name="smtpClientConfig">The <see cref="SmtpClientConfig"/> instance containing SMTP configuration settings.</param>
            /// <returns>A configured <see cref="SmtpClient"/> instance.</returns>
            /// <remarks>
            /// This allows seamless casting from <see cref="SmtpClientConfig"/> to <see cref="SmtpClient"/>:
            /// <code>
            /// var smtpConfig = new SmtpClientConfig("smtp.gmail.com", 587, "user@gmail.com", "password", true);
            /// using var smtpClient = (SmtpClient)smtpConfig;
            /// </code>
            /// </remarks>
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
