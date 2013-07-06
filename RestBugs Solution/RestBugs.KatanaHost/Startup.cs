using System.Web.Http;
using Owin;
using RestBugs.Services;

namespace RestBugs.KatanaHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            ServiceConfiguration.Configure(config);

            app.UseWebApi(config);
        }
    }
}