using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using Machine.Specifications;
using Microsoft.ApplicationServer.Http.Dispatcher;
using Moq;
using RestBugs.Services.Model;
using RestBugs.Services.Services;
using It = Machine.Specifications.It;

namespace RestBugs.Services.Specs
{
    public class when_posting_bug_to_team_member
    {
        Establish context = () =>
        {
            var mockBugRepository = new Mock<IBugRepository>();
            testBug = new Bug {Id=1};
            testBug.Activate(null);
            mockBugRepository.Setup(r => r.Get(1)).Returns(testBug);
            mockBugRepository.Setup(r => r.GetAll()).Returns(new[] {testBug});

            teamMember = new TeamMember { Id = 1, Name = "Howard Dierking" };
            var mockTeamRepository = new Mock<ITeamRepository>();
            mockTeamRepository.Setup(r => r.Get(1)).Returns(teamMember);

            service = new TeamService(mockBugRepository.Object, mockTeamRepository.Object);
        };

        Because of = () => {
            var data = new JsonObject();
            
            data["bugId"] = 1;
            data["teamMemberId"] = 1;
            data["comments"] = "my sample comments";

            result = service.PostBugToTeamMember(data).Content.ReadAs(); 
        };

        It should_be_in_the_team_members_list = () => result.ShouldContain(testBug);

        It should_add_new_item_in_bug_history = () => result.First().History.Count.ShouldEqual(3);

        It should_have_history_showing_1_field_changed = () => result.First().History[2].Item2.Count.ShouldEqual(1);

        It should_have_history_item_show_changed_assigned_to_field = () => result.First().History[2].Item2[0].Item1.ShouldEqual("Assigned To");

        It should_have_history_item_show_changed_assigned_to_value = () => result.First().History[2].Item2[0].Item2.ShouldEqual("Howard Dierking");

        It should_have_history_include_comments = () => result.First().History[2].Item3.ShouldEqual("my sample comments");

        static IEnumerable<Bug> result;
        static TeamMember teamMember;
        static TeamService service;
        static Bug testBug;
    }

    public class when_posting_bug_to_non_existant_team_member
    {
        Establish context = () =>
        {
            var mockBugRepository = new Mock<IBugRepository>();
            mockBugRepository.Setup(r => r.Get(1)).Returns(new Bug());

            var mockTeamRepository = new Mock<ITeamRepository>();
            mockTeamRepository.Setup(r => r.Get(1)).Returns(null as TeamMember);

            service = new TeamService(mockBugRepository.Object, mockTeamRepository.Object);
        };

        Because of = () => {
            var data = new JsonObject();
            data["bugId"] = 1;
            data["teamMemberId"] = 1;
            data["comments"] = null;

            Exception = Catch.Exception(() => service.PostBugToTeamMember(data));
        };

        It should_fail = () => Exception.ShouldNotBeNull();

        It should_fail_with_http_response_exception = () => Exception.ShouldBeOfType(typeof(HttpResponseException));

        It should_fail_with_404_status_code = () => ((HttpResponseException)Exception).Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);

        static Exception Exception;
        static TeamService service;
    }

    public class when_posting_inactive_bug_to_team_member
    {
        Establish context = () =>
        {
            var mockBugRepository = new Mock<IBugRepository>();
            mockBugRepository.Setup(r => r.Get(1)).Returns(new Bug());

            var mockTeamRepository = new Mock<ITeamRepository>();
            mockTeamRepository.Setup(r => r.Get(1)).Returns(new TeamMember { Id = 1, Name = "Howard Dierking" });

            service = new TeamService(mockBugRepository.Object, mockTeamRepository.Object);
        };

        Because of = () => {
            var data = new JsonObject();
            data["bugId"] = 1;
            data["teamMemberId"] = 1;
            data["comments"] = null;
            
            Exception = Catch.Exception(() => service.PostBugToTeamMember(data));
        };

        It should_fail = () => Exception.ShouldNotBeNull();

        It should_fail_with_http_response_exception = () => Exception.ShouldBeOfType(typeof(HttpResponseException));

        It should_fail_with_400_status_code = () => ((HttpResponseException)Exception).Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_fail_with_message = () => ((HttpResponseException)Exception).Response.Content.ReadAsString().ShouldEqual(
            "Cannot assign an inactive bug.");

        static Exception Exception;
        static TeamService service;
    }

    public class when_posting_non_existant_bug_to_team_member
    {
        Establish context = () =>
        {
            var mockBugRepository = new Mock<IBugRepository>();
            mockBugRepository.Setup(r => r.Get(1)).Returns(null as Bug);

            var mockTeamRepository = new Mock<ITeamRepository>();
            mockTeamRepository.Setup(r => r.Get(1)).Returns(new TeamMember { Id = 1, Name = "Howard Dierking" });

            service = new TeamService(mockBugRepository.Object, mockTeamRepository.Object);
        };

        Because of = () => {
            dynamic data = new JsonObject();
            data.bugId = 1;
            data.teamMemberId = 1;
            data.comments = null;

            Exception = Catch.Exception(() => service.PostBugToTeamMember(data));
        };

        It should_fail = () => Exception.ShouldNotBeNull();

        It should_fail_with_http_response_exception = () => Exception.ShouldBeOfType(typeof(HttpResponseException));

        It should_fail_with_400_status_code = () => ((HttpResponseException)Exception).Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_fail_with_message = () => ((HttpResponseException)Exception).Response.Content.ReadAsString().ShouldEqual(
            "Specified bug does not exist.");

        static Exception Exception;
        static TeamService service;
    }

    public class when_getting_team_member_bugs
    {
        Establish context = () =>
        {
            var mockBugRepository = new Mock<IBugRepository>();
            mockBugRepository.Setup(r => r.GetAll()).Returns(BugHelper.TestBugList);

            service = new TeamService(mockBugRepository.Object, new Mock<ITeamRepository>().Object);
        };

        Because of = () => { bugs = service.GetTeamMemberActiveBugs(1).Content.ReadAs(); };

        It should_return_2_bugs = () => bugs.Count().ShouldEqual(2);

        static IEnumerable<Bug> bugs;
        static TeamService service;
    }
}
