using System.ComponentModel.DataAnnotations;

namespace RestBugs.Services.Model
{
    public class BugDTO
    {
        public string Status { get; set; }

        public int Id { get; set; }

        public string AssignedTo { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public override bool Equals(object obj) {
            var cmp = obj as BugDTO;
            if(cmp == null)
                return false;

            return cmp.Status == Status
                   && cmp.AssignedTo == AssignedTo
                   && cmp.Title == Title
                   && cmp.Description == Description;

        }
    }
}
