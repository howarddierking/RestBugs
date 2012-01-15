using System;

namespace RestBugs.Services.Services
{
    using System.Web.Http;

    public class HomeController : ApiController
    {
        public SystemDescription Get() {
            return new SystemDescription();
        }
    }

    public class SystemDescription : ILinkProvider
    {
        public LinkInfo GetLinkInfo(string namedChild) {
            if(namedChild == null)
                throw new ArgumentException("Named child cannot be null for SystemDescription");
            switch (namedChild) {
                case "team":
                    return new LinkInfo("/team", "team members");
                case "activebugs":
                    return new LinkInfo("/bugs/active", "active bugs");
                case "resolvedbugs":
                    return new LinkInfo("/bugs/resolved", "resolved bugs");
                case "closedbugs":
                    return new LinkInfo("/bugs/closed", "closed bugs");
                case "pendingbugs":
                    return new LinkInfo("/bugs/pending", "pending (untriaged) bugs");
            }
            throw new ArgumentException("Unknown named child: " + namedChild);
        }
    }
}
