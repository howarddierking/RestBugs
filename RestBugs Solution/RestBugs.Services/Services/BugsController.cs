using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using RestBugs.Services.Infrastructure;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    using System.Json;
    using System.Web.Http;

    public class BugsController : ApiController
    {
        readonly IBugRepository _bugRepository;

        public BugsController(IBugRepository bugRepository) {
            _bugRepository = bugRepository;
        }

        public HttpResponseMessage<IEnumerable<Bug>> GetActive() {

            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Active)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank));

            response.SetTemplate("activebugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> GetResolved() {
            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Resolved)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank));

            response.SetTemplate("resolvedbugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> GetPending()
        {
            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Pending)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank));

            response.SetTemplate("pendingbugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> GetClosed()
        {
            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Closed)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank));

            response.SetTemplate("bugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> PostPending(HttpRequestMessage<JsonObject> requestMessage) {
            dynamic formData = requestMessage.Content.ReadAsync().Result;
            var bug = new Bug
            {
                Name = formData.name,
                Priority = formData.priority,
                Rank = formData.rank,
            };

            _bugRepository.Add(bug);

            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(b => b.Status == BugStatus.Pending)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank)) {StatusCode = HttpStatusCode.Created};

            response.Headers.Location = new Uri(string.Format("{0}/bug/{1}", HostURIFromRequest(requestMessage), bug.Id));

            response.SetTemplate("pendingbugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> PostActive(JsonObject content) {
            dynamic cnt = content;
            int bugId = cnt.id;
            string comments = cnt.comments;

            var bug = _bugRepository.Get(bugId);
            if (bug == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bug.Activate(comments);

            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Active)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank)) { StatusCode = HttpStatusCode.OK };

            response.SetTemplate("activebugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> PostResolved(JsonObject content)
        {
            dynamic cnt = content;
            int bugId = cnt.id;
            string comments = cnt.comments;

            var bug = _bugRepository.Get(bugId);
            if (bug == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bug.Resolve(comments);

            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Resolved)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank)) { StatusCode = HttpStatusCode.OK };

            response.SetTemplate("resolvedbugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> PostClosed(JsonObject content)
        {
            dynamic cnt = content;
            int bugId = cnt.id;
            string comments = cnt.comments;

            var bug = _bugRepository.Get(bugId);
            if (bug == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bug.Close(comments);

            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Closed)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank)) { StatusCode = HttpStatusCode.OK };

            response.SetTemplate("bugs");

            return response;
        }
        
        static string HostURIFromRequest(HttpRequestMessage requestMessage) {
            return string.Format("{0}://{1}:{2}", 
                requestMessage.RequestUri.Scheme, 
                requestMessage.RequestUri.Host,
                requestMessage.RequestUri.Port);
        }
    }
}
