using LockIn.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LockIn.Infrastructure.Data;

public sealed class LockInDbContext : DbContext
{
    public DbSet<DayTemplate> DayTemplates => Set<DayTemplate>();
    public DbSet<DayTemplateItem> DayTemplateItems => Set<DayTemplateItem>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanDay> PlanDays => Set<PlanDay>();
    public DbSet<PlanTask> PlanTasks => Set<PlanTask>();
    public DbSet<TaskTraining> TaskTrainings => Set<TaskTraining>();
    public DbSet<TaskTreadmill> TaskTreadmills => Set<TaskTreadmill>();
    public DbSet<TaskKeyActivity> TaskKeyActivities => Set<TaskKeyActivity>();
    public DbSet<TaskMedicationIntake> TaskMedicationIntakes => Set<TaskMedicationIntake>();
    public DbSet<KeyActivity> KeyActivities => Set<KeyActivity>();
    public DbSet<MedicationDose> MedicationDoses => Set<MedicationDose>();

    public LockInDbContext(DbContextOptions<LockInDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Converters
        var timeOnlyToString = new ValueConverter<TimeOnly, string>(
            v => v.ToString("HH\\:mm"), v => TimeOnly.ParseExact(v, "HH\\:mm"));
        var dateOnlyToString = new ValueConverter<DateOnly, string>(
            v => v.ToString("yyyy-MM-dd"), v => DateOnly.Parse(v));

        // DayTemplate
        b.Entity<DayTemplate>(e =>
        {
            e.ToTable("DayTemplate");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Version).IsRequired();
            e.Property(x => x.ParentTemplateId);
            e.Property(x => x.ConfirmedAt);
            e.Property(x => x.IsImmutable).IsRequired();
            e.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.Id, x.Version });
        });

        // DayTemplateItem
        b.Entity<DayTemplateItem>(e =>
        {
            e.ToTable("DayTemplateItem");
            e.HasKey(x => x.Id);
            e.Property(x => x.TemplateId).IsRequired();
            e.Property(x => x.TaskType).IsRequired();
            e.Property(x => x.StartTime).HasConversion(timeOnlyToString);
            e.Property(x => x.DisplayOrder).IsRequired();
            e.Property(x => x.MedicationDoseId);
            e.HasIndex(x => new { x.TemplateId, x.DisplayOrder }); // orden estable por plantilla
        });

        // Plan
        b.Entity<Entities.Plan>(e =>
        {
            e.ToTable("Plan");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100);
            e.Property(x => x.StartDate).HasConversion(dateOnlyToString).IsRequired();
            e.Property(x => x.EndDate).HasConversion(dateOnlyToString).IsRequired();
            e.Property(x => x.ConfirmedAt);
            e.Property(x => x.IsImmutable).IsRequired();
            e.HasIndex(x => new { x.StartDate, x.EndDate }); // consultas por rango
        });

        // PlanDay
        b.Entity<PlanDay>(e =>
        {
            e.ToTable("PlanDay");
            e.HasKey(x => x.Id);
            e.Property(x => x.PlanId).IsRequired();
            e.Property(x => x.Date).HasConversion(dateOnlyToString).IsRequired();
            e.HasIndex(x => new { x.PlanId, x.Date }).IsUnique(); // único por plan+fecha
            e.HasMany(x => x.Tasks).WithOne().HasForeignKey(x => x.PlanDayId).OnDelete(DeleteBehavior.Cascade);
        });

        // PlanTask
        b.Entity<Entities.PlanTask>(e =>
        {
            e.ToTable("PlanTask");
            e.HasKey(x => x.Id);
            e.Property(x => x.PlanDayId).IsRequired();
            e.Property(x => x.TaskType).IsRequired();
            e.Property(x => x.StartTime).HasConversion(timeOnlyToString);
            e.Property(x => x.DisplayOrder).IsRequired();
            e.Property(x => x.Status).IsRequired();
            // Snapshots (nullable según catálogo)
            e.Property(x => x.KeyActivityNameSnapshot).HasMaxLength(100);
            e.Property(x => x.MedicationNameSnapshot).HasMaxLength(100);
            e.Property(x => x.MedicationMgPerUnitSnapshot).HasPrecision(6, 1);
            e.Property(x => x.MedicationUnitsPerDoseSnapshot);
            e.HasIndex(x => new { x.PlanDayId, x.DisplayOrder }); // orden estable por día
        });

        // Subtipos 1:1
        b.Entity<TaskTraining>(e =>
        {
            e.ToTable("TaskTraining");
            e.HasKey(x => x.PlanTaskId);
            e.HasOne<Entities.PlanTask>().WithOne().HasForeignKey<TaskTraining>(x => x.PlanTaskId).OnDelete(DeleteBehavior.Cascade);
            e.Property(x => x.TrainingType).IsRequired();
        });

        b.Entity<TaskTreadmill>(e =>
        {
            e.ToTable("TaskTreadmill");
            e.HasKey(x => x.PlanTaskId);
            e.HasOne<Entities.PlanTask>().WithOne().HasForeignKey<TaskTreadmill>(x => x.PlanTaskId).OnDelete(DeleteBehavior.Cascade);
            e.Property(x => x.Km).HasPrecision(5, 2).IsRequired();
            e.Property(x => x.DurationMinutes).IsRequired();
        });

        b.Entity<TaskKeyActivity>(e =>
        {
            e.ToTable("TaskKeyActivity");
            e.HasKey(x => x.PlanTaskId);
            e.HasOne<Entities.PlanTask>().WithOne().HasForeignKey<TaskKeyActivity>(x => x.PlanTaskId).OnDelete(DeleteBehavior.Cascade);
            e.Property(x => x.KeyActivityId);
            e.Property(x => x.Minutes).IsRequired();
        });

        b.Entity<TaskMedicationIntake>(e =>
        {
            e.ToTable("TaskMedicationIntake");
            e.HasKey(x => x.PlanTaskId);
            e.HasOne<Entities.PlanTask>().WithOne().HasForeignKey<TaskMedicationIntake>(x => x.PlanTaskId).OnDelete(DeleteBehavior.Cascade);
            e.Property(x => x.MedicationDoseId);
            e.Property(x => x.UnitsTotal).IsRequired();
            e.Property(x => x.UnitsTaken).IsRequired();
        });

        // Catálogos
        b.Entity<KeyActivity>(e =>
        {
            e.ToTable("KeyActivity");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.Name).IsUnique(); // no estrictamente necesario, pero útil
        });

        b.Entity<MedicationDose>(e =>
        {
            e.ToTable("MedicationDose");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.MgPerUnit).HasPrecision(6, 1).IsRequired();
            e.Property(x => x.UnitsPerDose).IsRequired();
        });
    }
}
