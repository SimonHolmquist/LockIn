namespace LockIn.Infrastructure.Data.Entities;

public sealed class DayTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Version { get; set; }
    public Guid? ParentTemplateId { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public bool IsImmutable { get; set; }
    public List<DayTemplateItem> Items { get; set; } = new();
}
