//using System;
//using Hangfire;
//using Hangfire.SqlServer;
//using Microsoft.Owin;
//using Owin;

//[assembly: OwinStartup(typeof(Startup))]
//public class Startup
//{
//    public void Configuration(IAppBuilder app)
//    {
//        var options = new SqlServerStorageOptions
//        {
//            QueuePollInterval = TimeSpan.FromSeconds(30) // Default value
//        };
//        GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection", options);
//        app.UseHangfireServer();
//    }
//}
