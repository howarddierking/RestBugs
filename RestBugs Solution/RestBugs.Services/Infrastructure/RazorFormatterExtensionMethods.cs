using System.Net.Http;

namespace RestBugs.Services.Infrastructure
{
    public static class RazorFormatterExtensionMethods
    {
        public static void SetTemplate(this HttpResponseMessage response, string template)
        {
            response.Content.Headers.AddWithoutValidation("razortemplate", template);
        }
    }
}