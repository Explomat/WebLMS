using System.Data.Entity;

namespace WebLMS.Models
{
    public class WebLMSContext : DbContext
    {
        public WebLMSContext() : base("DefaultConnection") { }
        public DbSet<WebLMSForm> WebLMSForms { get; set; }
        public DbSet<File> Files { get; set; }
    }
}