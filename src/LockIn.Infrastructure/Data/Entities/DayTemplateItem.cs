using LockIn.Infrastructure.Data.Enums;

namespace LockIn.Infrastructure.Data.Entities;

public sealed class DayTemplateItem
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public TaskType TaskType { get; set; }
    public TimeOnly? StartTime { get; set; }
    public int DisplayOrder { get; set; }
    public Guid? MedicationDoseId { get; set; }
}
