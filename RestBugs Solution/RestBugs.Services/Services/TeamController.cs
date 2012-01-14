using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using RestBugs.Services.Model;

namespace RestBugs.Services.Services
{
    using System.Json;
    using System.Web.Http;

    public class TeamController : ApiController
    {
        readonly IBugRepository _bugRepository;
        readonly ITeamRepository _teamRepository;

        public TeamController(IBugRepository bugRepository, ITeamRepository teamRepository) {
            _bugRepository = bugRepository;
            _teamRepository = teamRepository;
        }

        public HttpResponseMessage<IEnumerable<TeamMember>> GetTeam() {
            var response =  new HttpResponseMessage<IEnumerable<TeamMember>>(_teamRepository.GetAll());

            response.Content.Headers.AddWithoutValidation("razortemplate", "team");
            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> GetTeamMemberActiveBugs(int teamMemberId) {
            var response =
                new HttpResponseMessage<IEnumerable<Bug>>(
                    _bugRepository.GetAll().Where(b => b.AssignedTo != null && b.AssignedTo.Id == teamMemberId));
            
            response.Content.Headers.AddWithoutValidation("razortemplate", "bugs");

            return response;
        }

        public HttpResponseMessage<IEnumerable<Bug>> PostBugToTeamMember(JsonObject requestData) {
            var bugId = requestData["bugId"].ReadAs<int>();
            var teamMemberId = requestData["teamMemberId"].ReadAs<int>();
            var comments = requestData["comments"] == null ? null : requestData["comments"].ReadAs<string>();

            var teamMember = _teamRepository.Get(teamMemberId);
            var bug = _bugRepository.Get(bugId);

            if (teamMember == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            if(bug == null) {
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Specified bug does not exist.")
                };
                throw new HttpResponseException(response);
            }

            if (bug.Status != BugStatus.Active) {   //todo: investigate how we refactor this into the domain while not bleeding http semantics into the domain
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Cannot assign an inactive bug.")
                };
                throw new HttpResponseException(response);
            }
            
            bug.AssignTo(teamMember, comments);
            
            return GetTeamMemberActiveBugs(teamMemberId);
        }
    }
}
