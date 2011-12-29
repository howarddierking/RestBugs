using System.Collections.Generic;
using RestBugs.Services.Model;

namespace RestBugs.Services.Specs
{
    public static class BugHelper
    {
        public static IEnumerable<Bug> TestBugList {
            get {
                var bugs = new List<Bug>
                {
                    new Bug {Status = BugStatus.Active, Priority = 3},
                    new Bug {Status = BugStatus.Active, Priority = 1, Rank = 2},
                    new Bug {Status = BugStatus.Active, Priority = 1},
                    new Bug {Status = BugStatus.Resolved},
                    new Bug {Status = BugStatus.Closed}
                };

                var howard = new TeamMember {Id = 1, Name = "Howard Dierking"};
                bugs[1].AssignTo(howard, null);
                bugs[2].AssignTo(howard, null);

                return bugs;
            }
        }
    }
}