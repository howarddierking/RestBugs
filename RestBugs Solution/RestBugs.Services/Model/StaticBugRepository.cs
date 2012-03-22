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
                    new Bug {Id= 1, Name = "Bug 1", Status = BugStatus.Working, Priority = 3},
                    new Bug {Id= 2, Name = "Bug 2", Status = BugStatus.Working, Priority = 1, Rank = 2},
                    new Bug {Id= 3, Name = "Bug 3", Status = BugStatus.Working, Priority = 1},
                    new Bug {Id= 4, Name = "Bug 4", Status = BugStatus.Done},
                    new Bug {Id= 5, Name = "Bug 5", Status = BugStatus.Done},
                    new Bug {Id= 6, Name = "Bug 6", Status = BugStatus.Backlog, Priority = 3},
                    new Bug {Id= 7, Name = "Bug 7", Status = BugStatus.Backlog, Priority = 1, Rank = 2},
                    new Bug {Id= 8, Name = "Bug 8", Status = BugStatus.Backlog, Priority = 1},
                };

            bugs = bugsList;
        }

        public IEnumerable<Bug> GetAll() {
            return bugs;
        }

        public Bug Get(int bugId) {
            return bugs.FirstOrDefault(b => b.Id == bugId);
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