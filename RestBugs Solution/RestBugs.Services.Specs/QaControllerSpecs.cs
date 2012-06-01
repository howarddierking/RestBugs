using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Moq;
using System.Net.Http;
using RestBugs.Services.Model;
using RestBugs.Services.Services;
using It = Machine.Specifications.It;

namespace RestBugs.Services.Specs
{
    public class when_getting_all_qa_bugs
    {
        Establish context = () =>
        {
            AutoMapperConfig.Configure();

            var mockRepo = new Mock<IBugRepository>();
            mockRepo.Setup(r => r.GetAll()).Returns(BugHelper.TestBugList);
            controller = new QaController(mockRepo.Object);
        };

        Because of = () => { controller.Get().TryGetContentValue<IEnumerable<BugDTO>>(out resolvedBugs); };

        It should_not_be_null = () => resolvedBugs.ShouldNotBeNull();

        It should_return_1_bug = () => resolvedBugs.Count().ShouldEqual(1);

        static IEnumerable<BugDTO> resolvedBugs;
        static QaController controller;
    }
}