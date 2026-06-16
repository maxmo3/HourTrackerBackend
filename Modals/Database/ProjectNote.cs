using System.ComponentModel.DataAnnotations.Schema;

namespace HourTrackerBackend.Modals.Database
{
    public class ProjectNote
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
