using System.ComponentModel.DataAnnotations;

namespace RestBugs.Services.Model
{
    public class BugDTO
    {
        [Required]
        public string Status { get; set; }

        public int Priority { get; set; }

        public decimal Rank { get; set; }

        public int Id { get; set; }

        public string AssignedTo { get; set; }

        public string Name { get; set; }
    }
}
