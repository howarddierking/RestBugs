using Machine.Specifications;
using RestBugs.Services.Model;

namespace RestBugs.Services.Specs
{
    class when_creating_a_new_bug
    {
        Because of = () => { bug = new Bug(); };
        
        It should_be_in_pending_state = () => bug.Status.ShouldEqual(BugStatus.Backlog);

        It should_have_1_entry_in_history = () => bug.History.Count.ShouldEqual(1);
        
        static Bug bug;
    }
}
