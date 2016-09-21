using System;

namespace WebLMS.Models
{
    public class WebLMSForm
    {
        public Int64 Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? IsQuickly { get; set; }
        public string Description { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
