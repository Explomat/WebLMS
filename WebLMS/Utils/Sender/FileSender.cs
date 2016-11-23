using System;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace WebLMS.Utils.Sender
{
    public class FileSender : ISender
    {
        public void SendFileLink(string pathTo, string link)
        {
            using (FileStream fs = File.Open(pathTo, FileMode.OpenOrCreate))
            {
                byte[] toBytes = Encoding.ASCII.GetBytes(link);
                fs.Write(toBytes, 0, toBytes.Length);
            }
        }
    }
}