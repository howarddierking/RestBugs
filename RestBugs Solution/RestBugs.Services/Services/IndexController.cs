using System.Diagnostics;
using System.Web.Http;

namespace RestBugs.Services.Services
{
    public class IndexController : ApiController
    {
        public object Get() {
            Trace.WriteLine("HIT");
            return null;
        }
    }
}
