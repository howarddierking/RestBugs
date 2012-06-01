using System;
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

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() => WriteStream(value, stream));
        }

        static void WriteStream(object value, Stream stream)
        {
            const string razorTemplate = "bugs-all"; //hard-coding for now...

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

        public override bool CanReadType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}