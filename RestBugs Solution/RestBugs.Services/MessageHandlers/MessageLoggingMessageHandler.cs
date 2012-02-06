using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestBugs.Services.MessageHandlers
{
public class MessageLoggingMessageHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestStream = new MemoryStream();
        return request.Content.CopyToAsync(requestStream).ContinueWith<Task<HttpResponseMessage>>(
            t => {
                if (t.Status != TaskStatus.RanToCompletion) {
                    TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
                    // Copy faulted or cancelled
                    return tcs.Task;
                }

                var requestStreamReader = new StreamReader(requestStream);
                requestStream.Position = 0;
                var readToEnd = requestStreamReader.ReadToEnd();
                Debug.WriteLine(readToEnd);

                // TODO: This is crappy, get NCL to help us clone HttpRequestMessage
                requestStream.Position = 0;
                request.Content = new StreamContent(requestStream);
                requestStream = null;

                return base.SendAsync(request, cancellationToken).ContinueWith(
                    tRes => {
                        var resp = tRes.Result;
                        var responseStream = new MemoryStream();
                        return resp.Content.CopyToAsync(responseStream).ContinueWith(
                            tResCopy => {
                                var responseStreamReader = new StreamReader(responseStream);
                                responseStream.Position = 0;
                                var responseText = responseStreamReader.ReadToEnd();
                                Debug.WriteLine(responseText);

                                responseStream.Position = 0;
                                resp.Content = new StreamContent(responseStream);
                                return resp;
                            });
                    }).Unwrap();
            }).Unwrap().ContinueWith(innerTask => {
                if (requestStream != null)
                    requestStream.Dispose();

                return innerTask;
            }).Unwrap();
    }
}
}