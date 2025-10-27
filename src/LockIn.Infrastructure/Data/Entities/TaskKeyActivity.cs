namespace LockIn.Infrastructure.Data.Entities;

public sealed class TaskKeyActivity
{
    public Guid PlanTaskId { get; set; }
    public Guid? KeyActivityId { get; set; } // puede eliminarse el catálogo
    public int Minutes { get; set; }
}
