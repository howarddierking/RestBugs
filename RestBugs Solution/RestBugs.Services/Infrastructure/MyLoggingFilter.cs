using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RestBugs.Services.Infrastructure
{
    public class MyLoggingFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Debug.WriteLine(actionContext.ActionDescriptor.ActionName + " started");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Debug.WriteLine(actionExecutedContext.ActionContext.ActionDescriptor.ActionName + " finished");
        }
    }
}
