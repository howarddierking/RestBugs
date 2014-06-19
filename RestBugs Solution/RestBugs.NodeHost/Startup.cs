using System;
using System.Web.Http;
using Owin;
using RestBugs.Services;

namespace RestBugs.NodeHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Required to load Razor templates
            string basedir = app.Properties["node.basedir"] as string;
            AppDomain.CurrentDomain.SetData("APPBASE", basedir);

            // Issue since v5.0.0-rc1? http://katanaproject.codeplex.com/workitem/81
            Microsoft.Owin.Infrastructure.SignatureConversions.AddConversions(app);

            // OWIN configuration
            var config = new HttpConfiguration();
            ServiceConfiguration.Configure(config);
            app.UseWebApi(config);
        }
    }
}