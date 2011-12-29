using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace RestBugs.Services.MessageHandlers
{
    public class EtagMessageHandler : DelegatingHandler
    {
        readonly IETagStore _eTagStore;

        public EtagMessageHandler() {
            Trace.WriteLine("EtagMessageHandler - Ctor");

            _eTagStore = new InMemoryETagStore();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken) {
            
            Trace.WriteLine("EtagMessageHandler - SendAsync");

            var taskFactory = new TaskFactory<HttpResponseMessage>();

            if (request.Method == HttpMethod.Get) {
                //if this is not a conditional get
                if (request.Headers.IfNoneMatch.Count == 0)
                    return base.SendAsync(request, cancellationToken).ContinueWith(task => {
                        Trace.WriteLine("EtagMessageHandler - Task - Not a conditional GET");

                        var resp = task.Result;
                        resp.Headers.ETag = new EntityTagHeaderValue(_eTagStore.Fetch(request.RequestUri));
                        return resp;
                    });

                //if this is not modified, stop processing the message and return a 304; make sure the etag is also set
                if (IfNoneMatchContainsStoredEtagValue(request))
                    
                    return taskFactory.StartNew(() =>
                    {
                        Trace.WriteLine("EtagMessageHandler - Task - Conditional GET match");

                        var resp = new HttpResponseMessage(HttpStatusCode.NotModified);
                        resp.Headers.ETag = new EntityTagHeaderValue(_eTagStore.Fetch(request.RequestUri));
                        return resp;
                    });

                //if this resource has been updated, let the request proceed to the operation and set a new etag on the return
                return base.SendAsync(request, cancellationToken).ContinueWith(task => { 
                    Trace.WriteLine("EtagMessageHandler - Task - Conditional GET not matched");

                    var resp = task.Result;
                    resp.Headers.ETag = new EntityTagHeaderValue(_eTagStore.Fetch(request.RequestUri));
                    return resp;
                });
            }

            if (request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
                //let the request processing continue; new etag value for resource; update response header
                if (IfMatchContainsStoredEtagValue(request))
                    return base.SendAsync(request, cancellationToken).ContinueWith(task => {
                        Trace.WriteLine("EtagMessageHandler - Task - Conditional update matched");

                        var resp = task.Result;
                        resp.Headers.ETag = new EntityTagHeaderValue(_eTagStore.UpdateETagFor(request.RequestUri));
                        return resp;
                    });

                    //stop processing and return a 412/precondition failed; update response header
                else 
                    return taskFactory.StartNew(() => {
                        Trace.WriteLine("EtagMessageHandler - Task - Conditional update not matched");

                        var resp = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
                        resp.Headers.ETag = new EntityTagHeaderValue(_eTagStore.Fetch(request.RequestUri));
                        return resp;
                    });

            //by default, let the request keep moving up the message handler stack
            return base.SendAsync(request, cancellationToken);
        }

        bool IfNoneMatchContainsStoredEtagValue(HttpRequestMessage request) {
            //we're going to assume that this handler will only care about 1 entity tag value for the specified resource
            // the one outstanding quesion here is whether or not the value of v.Tag will include the quotes
            if (request.Headers.IfNoneMatch.Count == 0)
                return false;

            return request.Headers.IfNoneMatch.Select(v => v.Tag).Contains(_eTagStore.Fetch(request.RequestUri));
        }

        bool IfMatchContainsStoredEtagValue(HttpRequestMessage request) {
            if (request.Headers.IfMatch.Count == 0) //if an if-match header is not sent, we're not going to impose anything
                return true;

            return request.Headers.IfMatch.Select(v => v.Tag).Contains(_eTagStore.Fetch(request.RequestUri));
        }
    }
}