namespace LockIn.Infrastructure.Data.Entities;

public sealed class TaskMedicationIntake
{
    public Guid PlanTaskId { get; set; }
    public Guid? MedicationDoseId { get; set; } // puede eliminarse el catálogo
    public int UnitsTotal { get; set; }
    public int UnitsTaken { get; set; }
}
