using System;
using System.Collections.Generic;

namespace RestBugs.Services.Model
{
    public class Bug
    {
        readonly List<Tuple<DateTime, List<Tuple<string, string>>, string>> _history;

        public Bug()
        {
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

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IList<Tuple<DateTime, List<Tuple<string, string>>, string>> History
        {
            get { return _history; }
        }

        public void Activate(string comments)
        {
            Status = BugStatus.Working;
            DateTime now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                             now, new List<Tuple<string, string>>{ 
                                 new Tuple<string, string>("Status", "Active")
                             }, comments));
        }

        public void Resolve(string comments)
        {
            Status = BugStatus.QA;
            DateTime now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                             now, new List<Tuple<string, string>>{ 
                                 new Tuple<string, string>("Status", "Resolved")
                             }, comments));
        }

        public void Approve()
        {
            Status = BugStatus.Backlog;
            var now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                now, new List<Tuple<string, string>>{
                    new Tuple<string, string>("Status", "Backlog")}, 
                    string.Empty ));
        }

        public void Close(string comments)
        {
            Status = BugStatus.Done;
            DateTime now = DateTime.Now;
            _history.Add(new Tuple<DateTime, List<Tuple<string, string>>, string>(
                             now, new List<Tuple<string, string>>{ 
                                 new Tuple<string, string>("Status", "Closed")
                             }, comments));
        }
    }
}