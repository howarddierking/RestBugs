using System.Collections.Generic;

namespace RestBugs.Services.Model
{
    public interface ITeamRepository {
        TeamMember Get(int teamMemberId);
        IEnumerable<TeamMember> GetAll();
    }
}