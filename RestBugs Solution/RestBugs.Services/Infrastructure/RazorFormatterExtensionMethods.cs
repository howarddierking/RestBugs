using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace RestBugs.Services.Infrastructure
{
    public static class RazorFormatterExtensionMethods
    {
        public static void SetTemplate(this HttpResponseMessage response, string template) {
            response.Content.Headers.AddWithoutValidation("razortemplate", template);
        }
    }
}
