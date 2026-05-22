using System.ComponentModel.DataAnnotations.Schema;

namespace HourTrackerBackend.Modals.Database;

public class ProjectMecanicLink
{
    public int Id { get; set; }

    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;
    public int ProjectId { get; set; }

    [ForeignKey("MechanicId")]
    public Mechanic Mechanic { get; set; } = null!;
    public int MechanicId { get; set; }

    [ForeignKey("WeekDataId")]
    public List<WeekData> WeekData { get; set; } = new List<WeekData>();
    public int WeekDataId { get; set; }

    public int? ProjectTypeId { get; set; }

    public DateTime Created { get; set; }
}
