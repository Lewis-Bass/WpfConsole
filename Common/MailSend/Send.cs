using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common.MailSend
{
	public class Send
	{

		//NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE
		// To use Google as an SMTP relay you need to lower the security on your account
		// This is done using the web site below
		// https://www.google.com/settings/security/lesssecureapps
		// This will resolve the error below
		// "Error in sending email" The SMTP server requires a secure connection or the client was not authenticated"
		//NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE NOTE

		#region Properties

		string _host;

		string _hostLogin;

		string _hostPassword;

		int _hostPort;

		string _fromEmailAddress;

		string _fromEmailName;

		public string Subject { get; set; }

		public string Body { get; set; }

		public Dictionary<string, byte[]> Attachments { get; set; } = new Dictionary<string, byte[]>();

		#endregion

		#region Constructor

		public Send(string host, string hostLogin, string hostPassword, int hostPort, string fromEmailAddress, string fromEmailName)
		{
			_host = host;
			_hostLogin = hostLogin;
			_hostPassword = hostPassword;
			_hostPort = hostPort;
			_fromEmailAddress = fromEmailAddress;
			_fromEmailName = fromEmailName;
		}
		#endregion

		public bool SendEmail(string[] toEmailAddress)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(_host) || string.IsNullOrWhiteSpace(_fromEmailAddress) || string.IsNullOrWhiteSpace(_fromEmailName) || _hostPort <= 0)
				{
					return false;
				}
				using (SmtpClient smtp = new SmtpClient())
				{
					smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
					smtp.UseDefaultCredentials = false;
					smtp.EnableSsl = true;
					smtp.Host = _host;
					smtp.Port = _hostPort;
					smtp.Credentials = new NetworkCredential(_hostLogin, _hostPassword);

					foreach (string toEmail in toEmailAddress)
					{
						var mail = new MailMessage()
						{
							From = new MailAddress(_fromEmailAddress, _fromEmailName),
							Subject = Subject,
							Body = Body,
							IsBodyHtml = true
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
			}
			catch (Exception ex)
			{
				Serilog.Log.Error(ex, "Could not send email");
				return false;
			}
			return true;
		}
	}
}
