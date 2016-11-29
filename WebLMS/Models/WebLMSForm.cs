using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLMS.Models
{
    [Table("WebLMSForm")]
    public class WebLMSForm
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? IsQuickly { get; set; }
        public string Description { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
