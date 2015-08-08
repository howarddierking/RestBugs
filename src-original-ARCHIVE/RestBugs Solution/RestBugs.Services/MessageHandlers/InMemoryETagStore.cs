using System;
using System.Collections.Generic;

namespace RestBugs.Services.MessageHandlers
{
    public class InMemoryETagStore : IETagStore
    {
        //todo: is there an existing thread safe dictionary that I can use here?
        static readonly Dictionary<Uri, Guid> EntityTagMap = new Dictionary<Uri, Guid>();

        public string Fetch(Uri resourceUri) {
            Guid newETagVal;

            if (!EntityTagMap.ContainsKey(resourceUri)) {
                newETagVal = Guid.NewGuid();
                EntityTagMap.Add(resourceUri, newETagVal);
            }
            else
                newETagVal = EntityTagMap[resourceUri];

            return Quote(newETagVal.ToString());
        }

        static string Quote(string val) {
            return string.Format("\"{0}\"", val);
        }

        public string UpdateETagFor(Uri requestUri) {
            var newETagVal = Guid.NewGuid();

            if (!EntityTagMap.ContainsKey(requestUri))
                EntityTagMap.Add(requestUri, newETagVal);
            else
                EntityTagMap[requestUri] = newETagVal;

            return Quote(newETagVal.ToString());
        }
    }
}