namespace LockIn.Infrastructure.Data.Entities;

public sealed class Plan
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public bool IsImmutable { get; set; }
}
