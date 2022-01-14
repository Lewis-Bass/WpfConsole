using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraries.MailSend
{
    public class Send
    {

        //NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE
        // To use Google as an smtp relay you need to lower the security on your account
        // This is done using the web site below
        // https://www.google.com/settings/security/lesssecureapps
        // This will resolve the error below
        // "Error in sending email" The SMTP server requires a secure connecton or the client was not authenticated"
        //NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE

        #region Properties

        public string Host { get; set; } = CommonLibraries.Properties.Resources.EmailHost;

        public string HostLogin { get; set; } = CommonLibraries.Properties.Resources.EmailHostLogin;

        public string HostPassword { get; set; } = CommonLibraries.Properties.Resources.EmailHostPassword;

        public int HostPort { get; set; } = int.Parse(CommonLibraries.Properties.Resources.EmailHostPort);
        public string FromEmailAddress { get; set; } = CommonLibraries.Properties.Resources.EmailFromAddress;

        public string FromEmailName { get; set; } = CommonLibraries.Properties.Resources.EmailFromName;

        public string Subject { get; set; }

        public string Body { get; set; }

        public Dictionary<string, byte[]> Attachments { get; set; } = new Dictionary<string, byte[]>();

        #endregion

        #region Constructor

        #endregion

        public bool SendEmail(string[] toEmailAddress)
        {
            if (string.IsNullOrWhiteSpace(Host) || string.IsNullOrWhiteSpace(FromEmailAddress) || string.IsNullOrWhiteSpace(FromEmailName) || HostPort <= 0)
            {
                return false; 
            }
            using (SmtpClient smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = Host;
                smtp.Port = HostPort;
                smtp.Credentials = new NetworkCredential(HostLogin, HostPassword);

                foreach (string toEmail in toEmailAddress)
                {
                    var mail = new MailMessage()
                    {
                        From = new MailAddress(FromEmailAddress, FromEmailName),
                        Subject = Subject,
                        Body = Body
                    };
                    foreach (var dictEntry in Attachments)
                    {
                        Attachment att = new Attachment(new MemoryStream(dictEntry.Value), dictEntry.Key);
                        mail.Attachments.Add(att);
                    }
                    mail.To.Add(new MailAddress(toEmail));

                    // send the email
                    smtp.Send(mail);
                }
            }

            return true;
        }
    }
}
