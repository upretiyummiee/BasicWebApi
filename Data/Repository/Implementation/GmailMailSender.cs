using BasicWebApi.Data.Repository.Interface;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BasicWebApi.Data.Repository.Implementation
{

    /// <summary>
    /// Not Working currently.
    /// </summary>

    public class GmailMailSender : IMailSender
    {
        public async Task SendMail(string email)
        {
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("SenderEmail", "Password");
                    client.Port = 587; // 25 587
                    client.Host = "smtp.gmail.com";
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("SenderEmail", "Prabesh Upreti");
                        mail.To.Add(new MailAddress(email));
                        mail.Subject = "A special subject";
                        mail.Body = "Sent From asp.net core web api";

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
