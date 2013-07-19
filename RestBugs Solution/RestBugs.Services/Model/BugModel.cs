using System.Collections.Generic;

namespace RestBugs.Services.Model
{
    class BugModel
    {
        private BugModel(){ /* NoOp */}
        public IEnumerable<BugDTO> Bugs { get; set; } 
        public BugsModelState ModelState { get; set; }

        // factories for model states
        public static BugModel Home()
        {
            return new BugModel {ModelState = BugsModelState.Home};
        }

        public static BugModel Collection(IEnumerable<BugDTO> bugs)
        {
            return new BugModel { ModelState = BugsModelState.Collection, Bugs = bugs};
        }
    }

    public enum BugsModelState
    {
        Home,
        Collection
    }
}
