using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class IndexController : ApiController
    {
        public object Get() {
            Trace.WriteLine("HIT");
            return new List<BugDTO>();
            return null;
        }
    }
}
