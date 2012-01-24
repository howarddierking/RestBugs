using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;
using Ninject;
using RestBugs.Services.Infrastructure;
using RestBugs.Services.MessageHandlers;
using RestBugs.Services.Model;

namespace RestBugs.Services
{
    public static class ServiceConfiguration
    {
        public static void Configure(HttpConfiguration config) {
            /* ======== URI Namespace ========
                / - GET
                /Team - GET | POST
                /Team/{Team member}/Bugs - GET | POST
                /Bugs/Active - GET | POST
                /Bugs/Resolved - GET | POST
                /Bugs/Closed - GET | POST
                /Bugs/Pending - GET | POST (adding new bug posts here)
                /Bug/{Bug} - GET | PUT | DELETE
                /Bug/{Bug}/Attachments - GET | POST | DELETE
                /Bug/{Bug}/History - GET
             */

            config.Routes.MapHttpRoute("active", "bugs/active", new { controller = "BugsActive" });
            config.Routes.MapHttpRoute("closed", "bugs/closed", new { controller = "BugsClosed" });
            config.Routes.MapHttpRoute("pending", "bugs/pending", new { controller = "BugsPending" });
            config.Routes.MapHttpRoute("resolved", "bugs/resolved", new { controller = "BugsResolved" });

            config.Routes.MapHttpRoute("defaultapi", "{controller}/{id}",
                                       new { controller = "Home", id = RouteParameter.Optional });
            
            config.Formatters.Add(new RazorHtmlMediaTypeFormatter());

            config.MessageHandlers.Add(new EtagMessageHandler());

            var kernel = new StandardKernel();
            kernel.Bind<IBugRepository>().To<StaticBugRepository>();
            kernel.Bind<ITeamRepository>().To<StaticTeamRepository>();

            config.ServiceResolver.SetResolver(t => kernel.Get(t), t => kernel.GetAll(t));
        }    
    }

    public class ChildResourceConstraint : IHttpRouteConstraint
    {
        public bool Match(HttpRequestMessage request, 
            IHttpRoute route, 
            string parameterName, 
            IDictionary<string, object> values, 
            HttpRouteDirection routeDirection) {

            var path = request.RequestUri.AbsolutePath;

            var pathSegments = path.Split(new[] {'/'});
            for (int i = 0; i < pathSegments.Length; i++) {
                if (pathSegments[i].Equals("bugs", StringComparison.InvariantCultureIgnoreCase) && i != pathSegments.Length - 1) {
                    var parentController = pathSegments[i];
                    var childController = pathSegments[i + 1];
                    var combinedController = string.Format("{0}{1}", parentController, childController);

                    values["controller"] = combinedController;
                    return true;
                }
            }

            return false;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) {
            var path = httpContext.Request.Url.AbsolutePath;

            var pathSegments = path.Split(new[] { '/' });
            for (int i = 0; i < pathSegments.Length; i++)
            {
                if (pathSegments[i].Equals("bugs", StringComparison.InvariantCultureIgnoreCase) && i != pathSegments.Length - 1)
                {
                    var parentController = pathSegments[i];
                    var childController = pathSegments[i + 1];
                    var combinedController = string.Format("{0}{1}", parentController, childController);

                    values["controller"] = combinedController;
                    return true;
                }
            }

            return false;
        }
    }
}
