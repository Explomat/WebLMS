using System;

namespace WebLMS.Models
{
    public class File
    {
        public Int64 Id { get; set; }
        public string Md5Hash { get; set; }
        public string FilePath { get; set; }
        public string EmailWhoConverted { get; set; }
    }
}