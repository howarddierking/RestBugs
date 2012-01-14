using System;
using System.Collections.Generic;

namespace RestBugs.Services.Model
{
    public class Bug : ILinkProvider
    {
        readonly List<Tuple<DateTime, List<Tuple<string, string>>, string>> _history;

        public Bug() {
            _history = new List<Tuple<DateTime, List<Tuple<string, string>>, string>>();
            
            DateTime now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                             now, new List<Tuple<string, string>>{ 
                                 new Tuple<string, string>("Status", "Pending")
                             }, "Created"));
        }

        public BugStatus Status { get; set; }

        public int Priority { get; set; }

        public decimal Rank { get; set; }

        public IList<Tuple<DateTime, List<Tuple<string, string>>, string>> History {
            get { return _history;  }
        }

        public int Id { get; set; }

        public TeamMember AssignedTo { get;  set; }

        public string Name { get; set; }

//todo: make a DTO for this since we don't want people setting this

        public void AssignTo(TeamMember teamMember, string comments) {
            var now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                             now,
                             new List<Tuple<string, string>> {
                                 new Tuple<string, string>("Assigned To", teamMember.Name)
                             }, comments));
            AssignedTo = teamMember;
        }

        public void Activate(string comments) {
            Status = BugStatus.Active;
            DateTime now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                             now, new List<Tuple<string, string>>{ 
                                 new Tuple<string, string>("Status", "Active")
                             }, comments));
        }

        public void Resolve(string comments) {
            Status = BugStatus.Resolved;
            DateTime now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                             now, new List<Tuple<string, string>>{ 
                                 new Tuple<string, string>("Status", "Resolved")
                             }, comments));
        }

        public void Close(string comments) {
            Status = BugStatus.Closed;
            DateTime now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                             now, new List<Tuple<string, string>>
                             {
                                 new Tuple<string, string>("Status", "Closed")
                             }, comments));
        }

        public LinkInfo GetLinkInfo(string namedChild = null)
        {
            if (namedChild == null)
                return new LinkInfo(string.Format("/bug/{0}", Id), "This bug");
            if (namedChild == "history")
                return new LinkInfo(string.Format("/bug/{0}/history", Id), "history");
            throw new ArgumentException("Cannot generate link for named child " + namedChild);
        }
    }
}