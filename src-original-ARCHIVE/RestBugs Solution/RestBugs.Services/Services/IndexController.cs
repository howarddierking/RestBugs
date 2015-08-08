using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class IndexController : ApiController
    {
        public BugModel Get() {
            Trace.WriteLine("HIT");
            return BugModel.Home();
        }
    }
}
