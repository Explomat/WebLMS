using System;
using System.IO;

namespace WebLMS.Utils
{
    public class StreamUtils
    {
        public static byte[] GetBytesFromStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static Stream GetStreamFromBytesArray(byte[] byteArray)
        {
            return new MemoryStream(byteArray);
        }
    }
}