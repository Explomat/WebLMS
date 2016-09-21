using System.Web;
using System.Web.Optimization;

namespace WebLMS
{
    public class BundleConfig 
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/header.css").Include("~/Content/theme.css").Include("~/Content/content.css").Include("~/Content/common.css").Include("~/Content/form-modal.css").Include("~/Content/video-converter.css"));
            bundles.Add(new ScriptBundle("~/Scripts/js").Include("~/Scripts/jquery1.12.4.js").Include("~/Scripts/jquery.form.js").Include("~/Scripts/bootstrap.js").Include("~/Scripts/formModal.js").Include("~/Scripts/mobileMenu.js").Include("~/Scripts/common.js"));
        }
    }
}