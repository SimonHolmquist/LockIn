namespace LockIn.Infrastructure.Data.Entities;

public sealed class TaskTreadmill
{
    public Guid PlanTaskId { get; set; }
    public decimal Km { get; set; } // precision 5,2
    public int DurationMinutes { get; set; }
}
