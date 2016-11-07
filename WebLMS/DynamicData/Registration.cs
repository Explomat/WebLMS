using System.Web.DynamicData;
using System.Web.Routing;
using WebLMS.Models;
using Microsoft.AspNet.DynamicData.ModelProviders;

namespace DynamicDataSample.DynamicData
{
    public class Registration
    {
        public static readonly MetaModel DefaultModel = new MetaModel();

        public static void Register(RouteCollection routes)
        {
            DefaultModel.RegisterContext(
                new EFDataModelProvider(() => new WebLMSContext()),
                new ContextConfiguration { ScaffoldAllTables = true });

            // This route must come first to prevent some other route from the site to take over
            routes.Insert(
                0,
                new DynamicDataRoute("dbadmin/{table}/{action}")
                {
                    Constraints = new RouteValueDictionary(new { action = "List|Details|Edit|Insert" }),
                    Model = DefaultModel
                });

            routes.MapPageRoute(
                "dd_default",
                "dbadmin/{pagename}",
                "~/DynamicData/Default.aspx");
        }
    }
}