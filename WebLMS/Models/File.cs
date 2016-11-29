using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLMS.Models
{
    [Table("File")]
    public class File
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public string Md5Hash { get; set; }
        public string FilePath { get; set; }
        public string EmailWhoConverted { get; set; }
        public DateTime Datetime { get; set; }
        public bool? IsDownloaded { get; set; }
    }
}