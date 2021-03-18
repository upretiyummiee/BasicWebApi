using BasicWebApi.Data.Repository.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace BasicWebApi.Data.Repository.Implementation
{


    /// <summary>
    /// Uses Outlook smtp service to send a mail.
    /// </summary>

    public class OutlookMailSender : IMailSender
    {
        private readonly ILogger<OutlookMailSender> _logger;

        public OutlookMailSender(ILogger<OutlookMailSender> logger)
        {
            _logger = logger;
        }

        public async Task SendMail(string email) 
        {
            try
            {
                using (SmtpClient client = new SmtpClient()) 
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("SenderEmail", "Password");
                    client.Port = 587; // 25 587
                    client.Host = "smtp.live.com";
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;

                    using (MailMessage mail = new MailMessage()) 
                    {
                        mail.From = new MailAddress("SenderEmail", "BasicWebApiDevelopment");
                        mail.To.Add(new MailAddress(email));
                        mail.Subject = "A special subject";
                        mail.Body = "Sent From asp.net core web api";


                        //////
                        ///

                        string file = "mytext.txt";

                        Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(file);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                        // Add the file attachment to this email message.
                        mail.Attachments.Add(data);


                        ////



                        await client.SendMailAsync(mail);
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }
    }
}
