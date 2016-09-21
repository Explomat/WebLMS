using System.Data.Entity;

namespace WebLMS.Models
{
    public class WebLMSContext : DbContext
    {
        public WebLMSContext() : base("WebLMS") { }
        public DbSet<WebLMSForm> WebLMSForms { get; set; }
    }
}