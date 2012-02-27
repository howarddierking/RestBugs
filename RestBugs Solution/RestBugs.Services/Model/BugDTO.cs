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
    }
}
