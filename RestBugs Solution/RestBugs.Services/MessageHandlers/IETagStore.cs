using System;

namespace RestBugs.Services.MessageHandlers
{
    public interface IETagStore
    {
        string Fetch(Uri resourceUri);
        string UpdateETagFor(Uri requestUri);
    }
}