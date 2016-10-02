using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using System.Web.Mvc;
using WebLMS.Controllers;
using System.Runtime.Remoting.Messaging;

namespace WebLMS.App_Start
{
    public class AsyncMvcRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new AsyncMvcHandler(requestContext);
        }

        class AsyncMvcHandler : IHttpAsyncHandler, IRequiresSessionState
        {
            RequestContext requestContext;
            WebLMS.Controllers.AsyncController asyncController;
            HttpContext httpContext;

            public AsyncMvcHandler(RequestContext context)
            {
                requestContext = context;
            }

            // IHttpHandler members
            public bool IsReusable { get { return false; } }
            public void ProcessRequest(HttpContext httpContext) { throw new NotImplementedException(); }

            // IHttpAsyncHandler members
            public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
            {
                // Get the controller type
                string controllerName = requestContext.RouteData.GetRequiredString("controller");

                // Obtain an instance of the controller
                IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
                IController controller = factory.CreateController(requestContext, controllerName);
                if (controller == null)
                    throw new InvalidOperationException("Can't locate the controller " + controllerName);
                try
                {
                    asyncController = controller as WebLMS.Controllers.AsyncController;
                    if (asyncController == null)
                        throw new InvalidOperationException("Controller isn't an AsyncController.");

                    // Set up asynchronous processing
                    httpContext = HttpContext.Current; // Save this for later
                    asyncController.Callback = cb;
                    //(asyncController as IController).Execute(new ControllerContext(requestContext, controller));
                    (asyncController as IController).Execute(new ControllerContext(requestContext, (controller as ControllerBase)).RequestContext);
                    return asyncController.Result;
                }
                finally
                {
                    factory.ReleaseController(controller);
                    //factory.DisposeController(controller);
                }
            }

            public void EndProcessRequest(IAsyncResult result)
            {
                CallContext.HostContext = httpContext; // So that RenderView() works
                asyncController.OnCompletion(result);
            }
        }
    }
}