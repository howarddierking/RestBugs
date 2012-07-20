using RestBugs.Services;
using System;
using System.Web.Http.SelfHost;

namespace RestBugs.ConsoleHost
{
    class Program
    {
        static void Main() {
            var config = new HttpSelfHostConfiguration(new Uri("http://localhost:9200"));

            ServiceConfiguration.Configure(config);
            
            var host = new HttpSelfHostServer(config);

            host.OpenAsync().Wait();

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            host.CloseAsync().Wait();
        }
    }
}