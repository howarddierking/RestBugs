using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RestBugs.Services.Infrastructure;
using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    public class WorkingController : ApiController
    {
        readonly IBugDtoRepository _bugDtoRepository;

        public WorkingController(IBugDtoRepository bugDtoRepository) {
            _bugDtoRepository = bugDtoRepository;
        }

        public HttpResponseMessage<IQueryable<BugDTO>>Get() {
            var response =
                new HttpResponseMessage<IQueryable<BugDTO>>(
                    _bugDtoRepository.GetAll()
                    .Where(b => b.Status == "Working")
                    .AsQueryable());

            return response;
        }

        public HttpResponseMessage<IEnumerable<BugDTO>> Post(int id, string comments)
        {
            BugDTO bugDto = _bugDtoRepository.Get(id); //bugId
            if (bugDto == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bugDto.Status = "Working";

            var response = new HttpResponseMessage<IEnumerable<BugDTO>>(
                _bugDtoRepository.GetAll().Where(b => b.Status == "Working")) {StatusCode = HttpStatusCode.OK};
            
            return response;
        }
    }
}