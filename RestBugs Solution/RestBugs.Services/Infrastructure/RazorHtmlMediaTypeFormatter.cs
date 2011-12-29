using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.IO;
using System.Net;

namespace RestBugs.Services.Infrastructure
{
    public class RazorHtmlMediaTypeFormatter : MediaTypeFormatter
    {
        public RazorHtmlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext context) {
            IEnumerable<string> headerValues;
            string razorTemplate = null;

            if (contentHeaders.TryGetValues("razortemplate", out headerValues))
                razorTemplate = headerValues.FirstOrDefault();

            if (razorTemplate != null)
                contentHeaders.Remove("razortemplate");

            var templateManager = new TemplateEngine();          
            var currentTemplate = templateManager.CreateTemplateForType(value.GetType(), razorTemplate);

            // set the model for the template
            currentTemplate.Model = value;
            currentTemplate.Execute();
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(currentTemplate.Buffer.ToString());
            }
            currentTemplate.Buffer.Clear();
        }

        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            throw new NotImplementedException();
        }
    }
}