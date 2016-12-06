using System;
using System.Net;
using System.Net.Mail;

namespace WebLMS.Utils.Sender
{
    public class EmailSender : ISender
    {
        public void SendFileLink(string pathTo, string link)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("info@weblms.ru"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(pathTo);

            //set the content 
            mail.Subject = String.Format("Заявка от {0}", pathTo);
            mail.Body = link;


            //send the message 
            SmtpClient smtp = new SmtpClient("mail.weblms.ru", 8889);

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential("info@weblms.ru", "1q2w3e4r5t6Y$");
            smtp.Credentials = Credentials;

            try
            {
                smtp.Send(mail);  
            }
            catch(Exception ex){}
                
        }
    }
}


//using GemBox.Email;
//using GemBox.Email.Security;
//using GemBox.Email.Smtp;
//using System;
//using System.Net.Security;
//using System.Security.Cryptography.X509Certificates;

//namespace WebLMS.Utils.Sender
//{
//    public class EmailSender : ISender
//    {
//        public void SendFileLink(string pathTo, string link)
//        {
//            using (SmtpClient smtp = new SmtpClient("mail.weblms.ru", 8889, ConnectionSecurity.Auto, null))
//            {
//                smtp.Connect();
//                smtp.Authenticate("info@weblms.ru", "1q2w3e4r5t6Y$");

//                MailMessage message = new MailMessage(new MailAddress(pathTo));
//                message.Subject = String.Format("Заявка от {0}", pathTo);
//                message.BodyText = link;

//                smtp.SendMessage(message);
//            }
//        }
//    }
//}