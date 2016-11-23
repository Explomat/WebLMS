using System;
using System.Net.Mail;

namespace WebLMS.Utils.Sender
{
    public class EmailSender : ISender
    {
        public void SendFileLink(string pathTo, string link)
        {
            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient("mail.weblms.ru");
            m.From = new MailAddress("info@weblms.ru");
            m.To.Add(pathTo);
            m.Subject = String.Format("Заявка от {0}", pathTo);
            m.Body = link;
            sc.Credentials = new System.Net.NetworkCredential("info@weblms.ru", "1q2w3e4r5t6Y$");
            sc.Send(m);
        }
    }
}