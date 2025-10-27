namespace LockIn.Infrastructure.Data.Entities;

public sealed class MedicationDose
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public decimal MgPerUnit { get; set; } // 6,1
    public int UnitsPerDose { get; set; }
}
