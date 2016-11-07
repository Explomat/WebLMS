using System.ComponentModel.DataAnnotations;
using System.Web.DynamicData;
using System.Web.UI.WebControls;
using System;
using System.Net;

namespace WebApplication2
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            /*if (!Page.User.Identity.IsAuthenticated)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                Response.End();
            }

            if (!Page.User.IsInRole("Admin"))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                Response.End();
            }*/

            base.OnInit(e);
        }
    }
}
