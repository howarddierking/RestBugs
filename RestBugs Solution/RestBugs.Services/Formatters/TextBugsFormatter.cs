using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using RestBugs.Services.Model;

namespace RestBugs.Services.Formatters
{
    public class TextBugsFormatter : BufferedMediaTypeFormatter
    {
        readonly MediaTypeHeaderValue _bugsV1HeaderValue;

        public TextBugsFormatter() {
            _bugsV1HeaderValue = new MediaTypeHeaderValue("text/vnd.howard.bugs");
            _bugsV1HeaderValue.Parameters.Add(new NameValueHeaderValue("Version","1.0"));
            SupportedMediaTypes.Add(_bugsV1HeaderValue);
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/vnd.howard.bugs"));
        }
        protected override bool CanWriteType(Type type) {
            var r =  typeof (Bug).IsAssignableFrom(type);
            return r;
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, 
            HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext) {
            var b = value as Bug;
         
            if (b == null) return;

            formatterContext.Response.Content.Headers.ContentType = _bugsV1HeaderValue;

            var writer = new StreamWriter(stream);
            writer.WriteLine("ID:{0}",b.Id);
            writer.WriteLine("Name:{0}",b.Name);
            writer.WriteLine("Priority:{0}",b.Priority);
            writer.WriteLine("Rank:{0}",b.Rank);

            writer.Flush();
        }

        protected override IEnumerable<KeyValuePair<string, string>> OnGetResponseHeaders(Type objectType, string mediaType, HttpResponseMessage responseMessage)
        {
            return new List<KeyValuePair<string, string>>()
                   {
                       new KeyValuePair<string, string>("content-type", _bugsV1HeaderValue.ToString())
                   };
        }
    }
}
