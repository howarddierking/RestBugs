using RestBugs.Services;

namespace RestBugs.ConsoleHost
{
    using System;
    using System.Web.Http.SelfHost;

    class Program
    {
        static void Main() {
            var config = new HttpSelfHostConfiguration(new Uri("http://localhost:8800/"));

            ServiceConfiguration.Configure(config);
            
            var host = new HttpSelfHostServer(config);

            host.OpenAsync().Wait();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            host.CloseAsync().Wait();
        }
    }
}