using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebLMS.Utils
{
    public class Hash
    {
        public static string GetMD5Hash(MD5 md5, Stream stream)
        {
            byte[] data = md5.ComputeHash(stream);
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}