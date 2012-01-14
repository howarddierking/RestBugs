namespace RestBugs.ConsoleHost
{
    using System;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using Ninject;
    using RestBugs.Services.Infrastructure;
    using RestBugs.Services.MessageHandlers;
    using RestBugs.Services.Model;

    class Program
    {
        static void Main() {
            var config = new HttpSelfHostConfiguration(new Uri("http://localhost:8800/"));

            //config.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());
            config.Formatters.Add(new RazorHtmlMediaTypeFormatter());

            config.MessageHandlers.Add(new EtagMessageHandler());

            //config.IncludeExceptionDetail = true;

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

            //var homeServiceBaseAddress = new Uri(systemBaseAddress, "");
            //var homeServiceHost = new HttpServiceHost(typeof(HomeController), GetConfig(), homeServiceBaseAddress);
            //homeServiceHost.Open();
            //Console.WriteLine("SystemServiceHost is open...");

            //var bugServiceBaseAddress = new Uri(systemBaseAddress, "bugs");
            //var bugServiceHost = new HttpServiceHost(typeof(BugsController), GetConfig(), bugServiceBaseAddress);
            //bugServiceHost.Open();
            //Console.WriteLine("BugServiceHost is open...");

            //var teamServiceBaseAddress = new Uri(systemBaseAddress, "team");
            //var teamServiceHost = new HttpServiceHost(typeof (TeamController), GetConfig(), teamServiceBaseAddress);
            //teamServiceHost.Open();
            //Console.WriteLine("TeamServiceHost is open...");
        }
    }
}