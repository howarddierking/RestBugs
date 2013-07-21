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

        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK, BugModel.Collection(GetResolvedDtos()));
            return response;
        }

        private IEnumerable<BugDTO> GetResolvedDtos()
        {
            var bugs = _bugRepository.GetAll().Where(b => b.Status == BugStatus.QA);
            var dtos = Mapper.Map<IEnumerable<Bug>, IEnumerable<BugDTO>>(bugs);
            return dtos;
        }

        public HttpResponseMessage Post(int id, string comments)
        {
            var bug = _bugRepository.Get(id); //bugId
            if (bug == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            bug.Resolve(comments);

            var response = Request.CreateResponse(HttpStatusCode.OK, BugModel.Collection(GetResolvedDtos()));
            return response;
        }
    }
}