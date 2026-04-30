namespace HourTrackerBackend.Modals.Request
{
    public class ProjectMessage
    {
        public string Name { get; set; } = null!;
        public string About { get; set; } = null!;
        public double EstimatedTimeInSeconds { get; set; } = 0;
        public bool MaterialsDelivered { get; set; }
        public List<ProjectTypeMessage>? Types { get; set; }
    }
}