using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace RestBugs.Services.Services
{
    public class IndexController : ApiController
    {
        public object Get() {
            //no op
            Trace.WriteLine("HIT");
            return null;
        }
    }
}
