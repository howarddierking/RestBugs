using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Machine.Fakes;
using Machine.Specifications;
using RestBugs.Services.Model;
using RestBugs.Services.Services;
using It = Machine.Specifications.It;

namespace RestBugs.Services.Specs
{
    [Subject(typeof(WorkingController))]
    public class when_getting_all_working_bugs : WithSubject<WorkingController>
    {
        Establish context = () => {
                                With(new DefaultControllerBehaviorConfig(Subject));

                                expectedResult = new List<BugDTO> {
                                    new BugDTO {Title = "Bug 3", Status = "Working"},
                                    new BugDTO {Title = "Bug 2", Status = "Working"},
                                    new BugDTO {Title = "Bug 1", Status = "Working"}
                                };
                            };

        Because of = () => Subject.Get().TryGetContentValue(out model);

        It should_not_be_null = () => model.Bugs.ShouldNotBeNull();

        It should_return_3_bugs = () => model.Bugs.Count().ShouldEqual(3);

        It should_sort_bugs_in_order_of_priority_then_rank = () => model.Bugs.ShouldEqual(expectedResult);

        static BugModel model;
        static IEnumerable<BugDTO> expectedResult;
    }

    [Subject(typeof(WorkingController))]
    public class when_posting_bug_to_working : WithSubject<WorkingController>
    {
        Establish context = () => {
                                With(new DefaultControllerBehaviorConfig(Subject));

                                var testBug = new Bug {Id = 1};

                                The<IBugRepository>()
                                    .WhenToldTo(r => r.Get(1))
                                    .Return(testBug);

                                The<IBugRepository>()
                                    .WhenToldTo(r => r.GetAll())
                                    .Return(new[] {testBug});
                            };

        Because of = () => {
                         responseMessage = Subject.Post(1, "activating bug 1");
                         result = responseMessage.TryGetContentValue(out model);
                     };

        It should_succeed_in_getting_result_content = () => result.ShouldBeTrue();

        It should_not_be_null_result = () => responseMessage.ShouldNotBeNull();

        It should_not_be_null_result_content = () => model.Bugs.ShouldNotBeNull();

        It should_have_1_bug_in_it = () => model.Bugs.Count().ShouldEqual(1);

        It should_contain_a_bug_with_id_1 = () => model.Bugs.First().Id.ShouldEqual(1);

        static HttpResponseMessage responseMessage;
        static BugModel model;
        static bool result;
    }

    [Subject(typeof(WorkingController))]
    public class when_posting_nonexistant_bug_to_working : WithSubject<WorkingController>
    {
        Establish context = () => {
                                AutoMapperConfig.Configure();

                                The<IBugRepository>()
                                    .WhenToldTo(r => r.Get(100))
                                    .Return(null as Bug);
                            };

        Because of = () => responseMessage = Subject.Post(100, "activating bug 1");

        It should_return_response_message = () => responseMessage.ShouldNotBeNull();

        It should_have_404_status_code = () => responseMessage.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        
        static HttpResponseMessage responseMessage;
    }
}