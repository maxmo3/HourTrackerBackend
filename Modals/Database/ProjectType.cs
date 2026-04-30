using System.ComponentModel.DataAnnotations.Schema;

namespace HourTrackerBackend.Modals.Database;

public class ProjectType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int ProjectId { get; set; }
    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;
    public DateTime Created { get; set; }
    public double CalculatedTimeInSeconds { get; set; } = 0;
}
