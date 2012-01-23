using System;
using System.Web.Http;

namespace RestBugs.Services.Services
{
    public class GreetingController : ApiController
    {
        [HttpGet]
        public string SayHi() {
            return "Hello World!";
        }
    }
}
