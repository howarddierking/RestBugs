using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class QaController : ApiController
    {
        readonly IBugRepository _bugRepository;

        public QaController(IBugRepository bugRepository) {
            _bugRepository = bugRepository;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>>Get()
        {
            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(GetResolvedDtos());
            return response;
        }

        private IEnumerable<BugDTO> GetResolvedDtos()
        {
            var bugs = _bugRepository.GetAll().Where(b => b.Status == BugStatus.QA);
            var dtos = Mapper.Map<IEnumerable<Bug>, IEnumerable<BugDTO>>(bugs);
            return dtos;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>> Post(int id, string comments)
        {
            var bug = _bugRepository.Get(id); //bugId
            if (bug == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bug.Resolve(comments);

            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(GetResolvedDtos())
                               {StatusCode = HttpStatusCode.OK};
            
            return response;
        }
    }
}