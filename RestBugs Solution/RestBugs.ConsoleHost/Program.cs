using System;
using Microsoft.ApplicationServer.Http;
using Ninject;
using RestBugs.Services.Infrastructure;
using RestBugs.Services.MessageHandlers;
using RestBugs.Services.Model;
using RestBugs.Services.Services;

namespace RestBugs.ConsoleHost
{
    class Program
    {
        static void Main() {
            var systemBaseAddress = new Uri("http://localhost:8800/");

            var homeServiceBaseAddress = new Uri(systemBaseAddress, "");
            var homeServiceHost = new HttpServiceHost(typeof(HomeService), GetConfig(), homeServiceBaseAddress);
            homeServiceHost.Open();
            Console.WriteLine("SystemServiceHost is open...");

            var bugServiceBaseAddress = new Uri(systemBaseAddress, "bugs");
            var bugServiceHost = new HttpServiceHost(typeof(BugsService), GetConfig(), bugServiceBaseAddress);
            bugServiceHost.Open();
            Console.WriteLine("BugServiceHost is open...");

            var teamServiceBaseAddress = new Uri(systemBaseAddress, "team");
            var teamServiceHost = new HttpServiceHost(typeof (TeamService), GetConfig(), teamServiceBaseAddress);
            teamServiceHost.Open();
            Console.WriteLine("TeamServiceHost is open...");

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            teamServiceHost.Close();
            bugServiceHost.Close();
            homeServiceHost.Close();
        }

        static HttpConfiguration GetConfig() {
            var kernel = new StandardKernel();
            kernel.Bind<IBugRepository>().To<StaticBugRepository>();
            kernel.Bind<ITeamRepository>().To<StaticTeamRepository>();

            var config = new HttpConfiguration();
            config.SetServiceInstanceProvider((t, ic, msg) => kernel.Get(t));
            config.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());
            config.Formatters.Add(new RazorHtmlMediaTypeFormatter());

            config.SetMessageHandlers(typeof(EtagMessageHandler));

            config.IncludeExceptionDetail = true;

            config.EnableHelpPage = true;

            return config;
        }
    }
}
