using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using RestBugs.Services.Infrastructure;

namespace RestBugs.Services.Formatters
{
    public class RazorMediaTypeFormatter<T> : MediaTypeFormatter
    {
        private string _template;

        public RazorMediaTypeFormatter(string template, params MediaTypeHeaderValue[] mediaTypes)
        {
            foreach (var mediaType in mediaTypes)
                SupportedMediaTypes.Add(mediaType);

            _template = template;
        }

        public override bool CanWriteType(Type type)
        {
            return (type == typeof(T) || type.IsSubclassOf(typeof(T)));
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() => WriteStream(value, writeStream));
        }

        void WriteStream(object value, Stream stream)
        {
            var templateManager = new TemplateEngine();

            var valType = value == null ? null : value.GetType();

            var currentTemplate = templateManager.CreateTemplateForType(valType, _template);

            // set the model for the template
            currentTemplate.Model = value;
            currentTemplate.Execute();

            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(currentTemplate.Buffer.ToString());
            streamWriter.Flush();

            currentTemplate.Buffer.Clear();
        }

        public override bool CanReadType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}