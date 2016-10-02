using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebLMS.Controllers
{
    public class AsyncController : Controller
    {
        internal AsyncCallback Callback { get; set; }
        internal IAsyncResult Result { get; set; }
        internal Action<IAsyncResult> OnCompletion { get; set; }

        protected void RegisterAsyncTask(Func<AsyncCallback, IAsyncResult> beginInvoke, Action<IAsyncResult> endInvoke)
        {
            OnCompletion = endInvoke;
            Result = beginInvoke(Callback);
        }
    }
}
