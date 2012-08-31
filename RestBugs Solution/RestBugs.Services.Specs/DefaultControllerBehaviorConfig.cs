using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using Machine.Fakes;
using RestBugs.Services.Model;

namespace RestBugs.Services.Specs
{
    public class DefaultControllerBehaviorConfig
    {
        public DefaultControllerBehaviorConfig(ApiController controller)
        {
            var request = new HttpRequestMessage();
            var cfg = new HttpConfiguration();
            request.Properties[HttpPropertyKeys.HttpConfigurationKey] = cfg;
            controller.Request = request;
        }

        //what type is fakeAccessor
        OnEstablish context = fakeAccessor =>
                                  {
                                      AutoMapperConfig.Configure();

                                      fakeAccessor.The<IBugRepository>()
                                          .WhenToldTo(r => r.GetAll())
                                          .Return(BugHelper.TestBugList);


                                  };
    }
}