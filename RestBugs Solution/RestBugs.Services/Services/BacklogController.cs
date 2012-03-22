using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class BacklogController : ApiController
    {
        readonly IBugRepository _bugsRepository;

        public BacklogController(IBugRepository bugsRepository) {
            _bugsRepository = bugsRepository;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>>Get() {
            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(GetBacklogBugDtos());
            return response;
        }

        private IEnumerable<BugDTO> GetBacklogBugDtos()
        {
            var bugs = _bugsRepository.GetAll().Where(b => b.Status == BugStatus.Backlog);
            var dtos = Mapper.Map<IEnumerable<Bug>, IEnumerable<BugDTO>>(bugs);
            return dtos;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>> Post (BugDTO dto, string comments)
        {
            Bug bug;
            if(dto.Id != 0)
            {
                bug = _bugsRepository.Get(dto.Id);
                if(bug == null)
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                bug.Approve();
                return new HttpResponseMessage<IEnumerable<BugDTO>>(GetBacklogBugDtos());
            }

            bug = Mapper.Map<BugDTO, Bug>(dto);
            bug.Approve();

            _bugsRepository.Add(bug);
            
            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(GetBacklogBugDtos(), HttpStatusCode.Created);
            
            //i still don't like this because it's fragmenting management of my links
            response.Headers.Location = new Uri(HostUriFromRequest(Request), bug.Id.ToString());

            return response;
        }

        static Uri HostUriFromRequest(HttpRequestMessage requestMessage)
        {
            return new Uri(String.Format("{0}://{1}:{2}",
                                         requestMessage.RequestUri.Scheme,
                                         requestMessage.RequestUri.Host,
                                         requestMessage.RequestUri.Port));
        }
    }
}