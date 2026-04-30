using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HourTrackerBackend.Modals.Database
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ProjectMecanicLink> Links { get; set; } = new List<ProjectMecanicLink>();
        public List<Todo> Todos { get; set; } = new List<Todo>();
        public List<ProjectType> Types { get; set; } = new List<ProjectType>();
        public string About { get; set; } = null!;
        public double EstimatedTimeInSeconds { get; set; }
        public int CommonId { get; set; }
        [ForeignKey("CommonId")]
        public Common Common { get; set; } = new Common();
        public DateTime Created { get; set; }
        public string? CreatedByUserName { get; set; }
        public bool MaterialsDelivered { get; set; }

        // Extra work approved by customer — billable, excluded from budget comparison
        public int MeerwerkSeconds { get; set; } = 0;

        // Hours the customer contributed themselves — reduces project burden
        public int DhzSeconds { get; set; } = 0;
    }
}
