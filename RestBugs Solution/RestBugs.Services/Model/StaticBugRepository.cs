using System.Collections.Generic;

using System.Linq;

namespace RestBugs.Services.Model
{
    public class StaticBugRepository : IBugRepository
    {
        static List<Bug> bugs;

        static StaticBugRepository() {
            var bugsList = new List<Bug>
                {
                    new Bug {Id= 1, Name = "Bug 1", Status = BugStatus.Active, Priority = 3},
                    new Bug {Id= 2, Name = "Bug 2", Status = BugStatus.Active, Priority = 1, Rank = 2},
                    new Bug {Id= 3, Name = "Bug 3", Status = BugStatus.Active, Priority = 1},
                    new Bug {Id= 4, Name = "Bug 4", Status = BugStatus.Resolved},
                    new Bug {Id= 5, Name = "Bug 5", Status = BugStatus.Closed}
                };

            var howard = new TeamMember { Id = 1, Name = "Howard Dierking" };
            bugsList[1].AssignTo(howard, null);
            bugsList[2].AssignTo(howard, null);

            bugs = bugsList;
        }

        public IEnumerable<Bug> GetAll() {
            return bugs;
        }

        public Bug Get(int bugId) {
            return bugs.Where(b => b.Id == bugId).FirstOrDefault();
        }

        public void Add(Bug bug) {
            bug.Id = GetNextId();
            bugs.Add(bug);
        }

        static int GetNextId() {
            return bugs.OrderBy(b => b.Id).Last().Id + 1;
        }
    }
}