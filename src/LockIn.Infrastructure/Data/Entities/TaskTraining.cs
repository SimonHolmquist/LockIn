using LockIn.Infrastructure.Data.Enums;

namespace LockIn.Infrastructure.Data.Entities;

public sealed class TaskTraining
{
    public Guid PlanTaskId { get; set; }
    public TrainingType TrainingType { get; set; }
}
