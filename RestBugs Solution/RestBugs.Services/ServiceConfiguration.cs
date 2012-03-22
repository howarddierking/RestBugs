using System.Web.Http;
using Ninject;
using RestBugs.Services.Formatters;
using RestBugs.Services.MessageHandlers;
using RestBugs.Services.Model;

namespace RestBugs.Services
{
    public static class ServiceConfiguration
    {
        public static void Configure(HttpConfiguration config) {

            config.Routes.MapHttpRoute("def", "bugs/{controller}", new {controller = "Index"});
          
            config.Formatters.Add(new RazorHtmlMediaTypeFormatter());
            config.MessageHandlers.Add(new EtagMessageHandler());

            var kernel = new StandardKernel();
            kernel.Bind<IBugRepository>().To<StaticBugRepository>();

            config.ServiceResolver.SetResolver(t => kernel.TryGet(t), t => kernel.GetAll(t));

            AutoMapperConfig.Configure();
        }    
    }
}
