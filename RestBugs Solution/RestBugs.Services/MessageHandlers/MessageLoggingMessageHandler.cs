using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestBugs.Services.MessageHandlers
{
public class MessageLoggingMessageHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                            CancellationToken cancellationToken) {
        using (var requestStream = new MemoryStream()) {
            request.Content.CopyToAsync(requestStream).Wait();
            using (var requestStreamReader = new StreamReader(requestStream)) {
                requestStream.Position = 0;
                var readToEnd = requestStreamReader.ReadToEnd();
                Debug.WriteLine(readToEnd);
            }
        }

        return base.SendAsync(request, cancellationToken).ContinueWith(task => {
            var resp = task.Result;
            using (var responseStream = new MemoryStream()) {
                resp.Content.CopyToAsync(responseStream).Wait();
                using (var responseStreamReader = new StreamReader(responseStream)) {
                    responseStream.Position = 0;
                    var readToEnd = responseStreamReader.ReadToEnd();
                    Debug.WriteLine(readToEnd);
                }
            }
            return resp;
        });
    }
}
}