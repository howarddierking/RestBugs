using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using RestBugs.Services.Infrastructure;

namespace RestBugs.Services.Formatters
{
    public class RazorHtmlMediaTypeFormatter : MediaTypeFormatter
    {
        public RazorHtmlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }

        protected override bool CanWriteType(Type type) {
            return true;
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() => WriteStream(value, stream, contentHeaders));
        }

        static void WriteStream(object value, Stream stream, HttpContentHeaders contentHeaders) {
            IEnumerable<string> headerValues;
            string razorTemplate = "bugs-all"; //hard-coding for now...

            //if (contentHeaders.TryGetValues("razortemplate", out headerValues))
            //    razorTemplate = headerValues.FirstOrDefault();

            //if (razorTemplate != null)
            //    contentHeaders.Remove("razortemplate");

            var templateManager = new TemplateEngine();

            var valType = value == null ? null : value.GetType();

            var currentTemplate = templateManager.CreateTemplateForType(valType, razorTemplate);

            // set the model for the template
            currentTemplate.Model = value;
            currentTemplate.Execute();

            using (var streamWriter = new StreamWriter(stream))
                streamWriter.Write(currentTemplate.Buffer.ToString());

            currentTemplate.Buffer.Clear();
        }
    }
}