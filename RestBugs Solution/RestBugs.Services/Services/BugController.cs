using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class BugController : ApiController
    {
        readonly IBugRepository _repository;

        public BugController(IBugRepository repository) {
            _repository = repository;
        }
        public Bug GetBug(int id) {
            return _repository.Get(id);
        }
    }
}
