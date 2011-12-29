using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http.Dispatcher;
using RestBugs.Services.Infrastructure;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    [ServiceContract]
    public class BugsService
    {
        readonly IBugRepository _bugRepository;

        public BugsService(IBugRepository bugRepository) {
            _bugRepository = bugRepository;
        }

        [WebGet(UriTemplate = "active")]
        public HttpResponseMessage<IEnumerable<Bug>> GetActive() {

            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Active)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank));

            response.SetTemplate("activebugs");

            return response;
        }

        [WebGet(UriTemplate = "resolved")]
        public HttpResponseMessage<IEnumerable<Bug>> GetResolved() {
            var response = new HttpResponseMessage<IEnumerable<Bug>>(
                _bugRepository.GetAll().Where(
                    b => b.Status == BugStatus.Resolved)
                    .OrderBy(b => b.Priority)
                    .ThenBy(b => b.Rank));

            response.SetTemplate("resolvedbugs");

            return response;
        }

        [WebGet(UriTemplate = "pending")]
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

        [WebGet(UriTemplate = "closed")]
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

        [WebInvoke(UriTemplate = "pending", Method="POST")]
        public HttpResponseMessage<IEnumerable<Bug>> PostPending(HttpRequestMessage<JsonObject> requestMessage) {
            dynamic formData = requestMessage.Content.ReadAs();
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

        [WebInvoke(UriTemplate = "active", Method = "POST")]
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

        [WebInvoke(UriTemplate = "resolved", Method = "POST")]
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

        [WebInvoke(UriTemplate = "closed", Method = "POST")]
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
