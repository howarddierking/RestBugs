using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Machine.Specifications;
using Moq;
using RestBugs.Services.Model;
using RestBugs.Services.Services;
using It = Machine.Specifications.It;

namespace RestBugs.Services.Specs
{
    public class when_getting_all_working_bugs
    {
        Establish context = () =>
        {
            AutoMapperConfig.Configure();

            var mockRepo = new Mock<IBugRepository>();
            mockRepo.Setup(r => r.GetAll()).Returns(BugHelper.TestBugList);
            controller = new WorkingController(mockRepo.Object);

            expectedResult = new List<BugDTO>
                             {
                                 new BugDTO {Title="Bug 1", Status = "Working"},
                                 new BugDTO {Title="Bug 2", Status = "Working"},
                                 new BugDTO {Title="Bug 3", Status = "Working"}
                             };
        };

        Because of = () => { workingBugs = controller.Get().Content.ReadAsync().Result; };

        It should_not_be_null = () => workingBugs.ShouldNotBeNull();

        It should_return_3_bugs = () => workingBugs.Count().ShouldEqual(3);

        It should_sort_bugs_in_order_of_priority_then_rank = () => workingBugs.SequenceEqual(expectedResult);

        static IEnumerable<BugDTO> workingBugs;
        static IEnumerable<BugDTO> expectedResult;
        static WorkingController controller;
    }

    public class when_posting_bug_to_working
    {
        Establish context = () =>
        {
            AutoMapperConfig.Configure();

            var testBug = new Bug { Id = 1 };
            var mockRepo = new Mock<IBugRepository>();
            mockRepo.Setup(r => r.Get(1)).Returns(testBug);
            mockRepo.Setup(r => r.GetAll()).Returns(new[] { testBug });

            controller = new WorkingController(mockRepo.Object);
        };

        Because of = () =>
        {
            result = controller.Post(1, "activating bug 1");
            resultContent = result.Content.ReadAsync().Result;
        };

        It should_have_enum_dto_instance = () => {
            result.TryGetContentValue<IEnumerable<BugDTO>>(out val).ShouldBeTrue();
        };

        It should_not_be_null = () => result.ShouldNotBeNull();

        It should_have_1_bug_in_it = () => resultContent.Count().ShouldEqual(1);

        It should_contain_a_bug_with_id_1 = () => resultContent.First().Id.Equals(1);

        static WorkingController controller;
        static HttpResponseMessage result;
        static IEnumerable<BugDTO> resultContent;
    }

    public class when_posting_nonexistant_bug_to_working
    {
        Establish context = () =>
        {
            AutoMapperConfig.Configure();

            var mockRepo = new Mock<IBugRepository>();
            mockRepo.Setup(r => r.Get(100)).Returns(null as Bug);
            controller = new WorkingController(mockRepo.Object);
        };

        Because of = () => Exception = Catch.Exception(() => controller.Post(100, "activating bug 1"));

        It should_fail = () => Exception.ShouldNotBeNull();

        It should_be_http_exception = () => Exception.ShouldBeOfType(typeof(HttpResponseException));

        It should_throw_http_404 = () => ((HttpResponseException)Exception).Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);

        static WorkingController controller;
        static Exception Exception;
    }
}