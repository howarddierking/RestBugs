using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Moq;
using RestBugs.Services.Model;
using RestBugs.Services.Services;
using It = Machine.Specifications.It;

namespace RestBugs.Services.Specs
{
    public class when_getting_all_resolved_bugs
    {
        Establish context = () =>
        {
            var mockRepo = new Mock<IBugRepository>();
            mockRepo.Setup(r => r.GetAll()).Returns(BugHelper.TestBugList);
            controller = new BugsResolvedController(mockRepo.Object);
        };

        Because of = () => { resolvedBugs = controller.Get().Content.ReadAsync().Result; };

        It should_not_be_null = () => resolvedBugs.ShouldNotBeNull();

        It should_return_1_bug = () => resolvedBugs.Count().ShouldEqual(1);

        static IEnumerable<Bug> resolvedBugs;
        static BugsResolvedController controller;
    }
}