using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RestBugs.Services.Infrastructure;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class QaController : ApiController
    {
        readonly IBugDtoRepository _bugDtoRepository;

        public QaController(IBugDtoRepository bugDtoRepository) {
            _bugDtoRepository = bugDtoRepository;
        }

        public HttpResponseMessage<IQueryable<BugDTO>>Get() {
            var response =
                new HttpResponseMessage<IQueryable<BugDTO>>(
                    _bugDtoRepository.GetAll()
                    .Where(b => b.Status == "QA")
                    .AsQueryable());

            return response;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>> Post(int id, string comments)
        {
            BugDTO bugDto = _bugDtoRepository.Get(id); //bugId
            if (bugDto == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bugDto.Status = "QA";

            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(
                _bugDtoRepository.GetAll().Where(b => b.Status == "QA")) {StatusCode = HttpStatusCode.OK};
            
            return response;
        }
    }
}