using System;
using System.Collections.Generic;
using System.Linq;

namespace RestBugs.Services.Model
{
    public class StaticTeamRepository : ITeamRepository {
        static List<TeamMember> team;

        static StaticTeamRepository() {
            var t = new List<TeamMember>
            {
                new TeamMember {Id = 1, Name = "Howard Dierking"},
                new TeamMember {Id = 2, Name = "Glenn Block"},
                new TeamMember {Id = 3, Name = "Scott Hanselman"}
            };

           team = t;
        }

        public TeamMember Get(int teamMemberId) {
            return team.Where(t => t.Id == teamMemberId).FirstOrDefault();
        }

        public IEnumerable<TeamMember> GetAll() {
            return team;
        }
    }
}