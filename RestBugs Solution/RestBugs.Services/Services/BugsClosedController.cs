using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RestBugs.Services.Infrastructure;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class BugsClosedController : ApiController
    {
        readonly IBugRepository _bugRepository;

        public BugsClosedController(IBugRepository bugRepository) {
            _bugRepository = bugRepository;
        }

        public HttpResponseMessage<IEnumerable<Bug>> Get() {
            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Closed)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank));

            response.SetTemplate("bugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> Post(JsonValue content) {
            dynamic cnt = content;
            int bugId = cnt.id;
            string comments = cnt.comments;

            Bug bug = _bugRepository.Get(bugId);
            if (bug == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bug.Close(comments);

            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Closed)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank)) {StatusCode = HttpStatusCode.OK};

            response.SetTemplate("bugs");

            return response;
        }
    }
}