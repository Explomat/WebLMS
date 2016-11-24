using GemBox.Email;
using GemBox.Email.Security;
using GemBox.Email.Smtp;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WebLMS.Utils.Sender
{
    public class EmailSender : ISender
    {
        public void SendFileLink(string pathTo, string link)
        {
            using (SmtpClient smtp = new SmtpClient("mail.weblms.ru"))
            {
                smtp.Connect();
                smtp.Authenticate("info@weblms.ru", "1q2w3e4r5t6Y$");

                MailMessage message = new MailMessage(new MailAddress(pathTo));
                message.Subject = String.Format("Заявка от {0}", pathTo);
                message.BodyText = link;

                smtp.SendMessage(message);
            }
        }
    }
}