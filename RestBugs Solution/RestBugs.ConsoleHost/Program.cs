namespace RestBugs.ConsoleHost
{
    using System;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using Ninject;
    using Services.Infrastructure;
    using Services.MessageHandlers;
    using Services.Model;

    class Program
    {
        static void Main() {
            var config = new HttpSelfHostConfiguration(new Uri("http://localhost:8800/"));
            
            
            //config.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());
            config.Formatters.Add(new RazorHtmlMediaTypeFormatter());

            //config.MessageHandlers.Add(new EtagMessageHandler());
            //config.IncludeExceptionDetail = true;

            /*
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
            config.Routes.MapHttpRoute("default", "{controller}", new { controller = "Home" });

            var kernel = new StandardKernel();
            kernel.Bind<IBugRepository>().To<StaticBugRepository>();
            kernel.Bind<ITeamRepository>().To<StaticTeamRepository>();

            config.ServiceResolver.SetResolver(t => kernel.Get(t), t => kernel.GetAll(t));

            var host = new HttpSelfHostServer(config);

            host.OpenAsync().Wait();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            host.CloseAsync().Wait();
        }
    }
}