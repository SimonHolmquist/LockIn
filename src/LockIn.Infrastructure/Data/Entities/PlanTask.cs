using LockIn.Infrastructure.Data.Enums;

namespace LockIn.Infrastructure.Data.Entities;

public sealed class PlanTask
{
    public Guid Id { get; set; }
    public Guid PlanDayId { get; set; }
    public TaskType TaskType { get; set; }
    public TimeOnly? StartTime { get; set; }
    public int DisplayOrder { get; set; }
    public Enums.TaskStatus Status { get; set; }

    // Snapshots
    public string? KeyActivityNameSnapshot { get; set; }
    public string? MedicationNameSnapshot { get; set; }
    public decimal? MedicationMgPerUnitSnapshot { get; set; }
    public int? MedicationUnitsPerDoseSnapshot { get; set; }
}
