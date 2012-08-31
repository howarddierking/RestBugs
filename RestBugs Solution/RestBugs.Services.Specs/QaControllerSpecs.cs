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
    [Subject(typeof(QaController))]
    public class when_getting_all_qa_bugs : WithSubject<QaController>
    {
        Establish context = () => With(new DefaultControllerBehaviorConfig(Subject));

        Because of = () => Subject.Get().TryGetContentValue(out resolvedBugs);

        It should_not_be_null = () => resolvedBugs.ShouldNotBeNull();

        It should_return_1_bug = () => resolvedBugs.Count().ShouldEqual(1);

        static IEnumerable<BugDTO> resolvedBugs;
    }
}