using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RestBugs.Services.Infrastructure;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class BacklogController : ApiController
    {
        readonly IBugDtoRepository _bugDtoRepository;

        public BacklogController(IBugDtoRepository bugDtoRepository) {
            _bugDtoRepository = bugDtoRepository;
        }

        public HttpResponseMessage<IQueryable<BugDTO>>Get() {
            var response =
                new HttpResponseMessage<IQueryable<BugDTO>>(
                    _bugDtoRepository.GetAll()
                    .Where(b => b.Status == "Backlog")
                    .AsQueryable());

            return response;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>> Post(BugDTO bugDto)
        {
            if (bugDto.Id == 0)
                _bugDtoRepository.Add(bugDto);
            else
                bugDto = _bugDtoRepository.Get(bugDto.Id); //this is terrible as it cause data loss of input

            bugDto.Status = "Backlog";

            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(
                _bugDtoRepository.GetAll().Where(b => b.Status == "Backlog")) {StatusCode = HttpStatusCode.OK};
            
            return response;
        }

        static string HostUriFromRequest(HttpRequestMessage requestMessage)
        {
            return String.Format("{0}://{1}:{2}",
                                 requestMessage.RequestUri.Scheme,
                                 requestMessage.RequestUri.Host,
                                 requestMessage.RequestUri.Port);
        }
    }
}