using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class DoneController : ApiController
    {
        readonly IBugRepository _bugRepository;

        public DoneController(IBugRepository bugRepository) {
            _bugRepository = bugRepository;
        }

        private IEnumerable<BugDTO> GetDoneBugDtos()
        {
            var bugs = _bugRepository.GetAll().Where(b => b.Status == BugStatus.Done);
            var dtos = Mapper.Map<IEnumerable<Bug>, IEnumerable<BugDTO>>(bugs);
            return dtos;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>>Get()
        {
            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(GetDoneBugDtos());
            return response;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>> Post(int id, string comments)
        {
            var bug = _bugRepository.Get(id); //bugId
            if (bug == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bug.Close(comments);

            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(GetDoneBugDtos())
                               {StatusCode = HttpStatusCode.OK};
            
            return response;
        }
    }
}