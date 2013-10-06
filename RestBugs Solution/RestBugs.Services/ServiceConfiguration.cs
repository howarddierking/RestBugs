using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web.Http;
using Ninject;
using RestBugs.Services.Formatters;
using RestBugs.Services.Model;
using RestBugs.Services.Infrastructure;
using System.Web.Http.Controllers;

namespace RestBugs.Services
{
    public static class ServiceConfiguration
    {
        public static void Configure(HttpConfiguration config) {

            config.Routes.MapHttpRoute("def", "bugs/{controller}", new { controller = "Index" });
            config.Formatters.Clear();
            config.Formatters.Add(new RazorMediaTypeFormatter<BugModel>("bugs-all", new MediaTypeHeaderValue("text/html")));
            config.Formatters.Add(new RazorMediaTypeFormatter<BugModel>("bugs-all-json", new MediaTypeHeaderValue("application/json"), new MediaTypeHeaderValue("text/json")));

            //config.MessageHandlers.Add(new EtagMessageHandler());

            var kernel = new StandardKernel();
            kernel.Bind<IBugRepository>().To<StaticBugRepository>();
            kernel.Bind<IActionValueBinder>().To<MvcActionValueBinder>();

            config.DependencyResolver = new NinjectDependencyResolver(kernel);

            AutoMapperConfig.Configure();
        }    
    }
}
