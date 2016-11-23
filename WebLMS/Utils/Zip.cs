using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace WebLMS.Utils
{
    public class Zip
    {
        public static void Unzip(Stream inputStream, string destDirectory)
        {
            using (ZipStorer zip = ZipStorer.Open(inputStream, FileAccess.Read))
            {
                // Read the central directory collection
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    zip.ExtractFile(entry, Path.Combine(destDirectory, entry.FilenameInZip));
                }
            }
        }
    }
}