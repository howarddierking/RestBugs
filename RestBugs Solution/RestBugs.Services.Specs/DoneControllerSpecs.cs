using System.Collections.Generic;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using System.Net.Http;
using RestBugs.Services.Model;
using RestBugs.Services.Services;
using It = Machine.Specifications.It;

namespace RestBugs.Services.Specs
{
    [Subject(typeof(DoneController))]
    public class when_getting_all_done_bugs : WithSubject<DoneController>
    {
        Establish context = () => With(new DefaultControllerBehaviorConfig(Subject));

        Because of = () => Subject.Get().TryGetContentValue(out model);

        It should_not_be_null = () => model.Bugs.ShouldNotBeNull();

        It should_return_1_bug = () => model.Bugs.Count().ShouldEqual(1);

        static BugModel model;
    }
}