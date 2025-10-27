namespace LockIn.Infrastructure.Data.Entities;

public sealed class PlanDay
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public DateOnly Date { get; set; }
    public List<PlanTask> Tasks { get; set; } = new();
}
