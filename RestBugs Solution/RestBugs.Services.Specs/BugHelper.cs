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
                    new Bug {Name = "Bug 1", Status = BugStatus.Working, Priority = 3},
                    new Bug {Name = "Bug 2", Status = BugStatus.Working, Priority = 1, Rank = 2},
                    new Bug {Name = "Bug 3", Status = BugStatus.Working, Priority = 1},
                    new Bug {Name = "Bug 4", Status = BugStatus.Done},
                    new Bug {Name = "Bug 5", Status = BugStatus.Backlog},
                    new Bug {Name = "Bug 6", Status = BugStatus.QA}
                };

                return bugs;
            }
        }
    }
}