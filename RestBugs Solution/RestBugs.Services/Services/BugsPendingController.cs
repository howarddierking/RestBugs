using System;
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
    public class BugsPendingController : ApiController
    {
        readonly IBugRepository _bugRepository;

        public BugsPendingController(IBugRepository bugRepository) {
            _bugRepository = bugRepository;
        }

        public HttpResponseMessage<IEnumerable<Bug>> Get() {
            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Pending)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank));

            response.SetTemplate("pendingbugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> Post(JsonValue formData) {
            var data = formData.AsDynamic();
            var bug = new Bug
                        {
                            Name = data.name,
                            Priority = data.priority,
                            Rank = data.rank,
                        };

            _bugRepository.Add(bug);

            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(b => b.Status == BugStatus.Pending)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank)) {StatusCode = HttpStatusCode.Created};

            response.Headers.Location = new Uri(String.Format("{0}/bug/{1}", HostUriFromRequest(Request), bug.Id));

            response.SetTemplate("pendingbugs");

            return response;
        }

        static string HostUriFromRequest(HttpRequestMessage requestMessage) {
            return String.Format("{0}://{1}:{2}",
                                 requestMessage.RequestUri.Scheme,
                                 requestMessage.RequestUri.Host,
                                 requestMessage.RequestUri.Port);
        }
    }
}