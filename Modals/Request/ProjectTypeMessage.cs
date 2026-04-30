namespace HourTrackerBackend.Modals.Request;

public class ProjectTypeMessage
{
    public string Name { get; set; } = null!;
    public double CalculatedTimeInSeconds { get; set; } = 0;
}
