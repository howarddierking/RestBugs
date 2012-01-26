using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class TestBugController: ApiController
    {
        public BugDTO Post(BugDTO bugDto) {
            //var bugDto = new BugDTO();
            bugDto.Name = string.Concat(bugDto.Name, "other stuff");
            return bugDto;
        }
    }
}
