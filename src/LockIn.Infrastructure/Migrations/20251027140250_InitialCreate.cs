using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockIn.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DayTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentTemplateId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    IsImmutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicationDose",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MgPerUnit = table.Column<decimal>(type: "TEXT", precision: 6, scale: 1, nullable: false),
                    UnitsPerDose = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDose", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    StartDate = table.Column<string>(type: "TEXT", nullable: false),
                    EndDate = table.Column<string>(type: "TEXT", nullable: false),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    IsImmutable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanDay",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanDay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DayTemplateItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TaskType = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    MedicationDoseId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayTemplateItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DayTemplateItem_DayTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "DayTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlanDayId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TaskType = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    KeyActivityNameSnapshot = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MedicationNameSnapshot = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MedicationMgPerUnitSnapshot = table.Column<decimal>(type: "TEXT", precision: 6, scale: 1, nullable: true),
                    MedicationUnitsPerDoseSnapshot = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanTask_PlanDay_PlanDayId",
                        column: x => x.PlanDayId,
                        principalTable: "PlanDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskKeyActivity",
                columns: table => new
                {
                    PlanTaskId = table.Column<Guid>(type: "TEXT", nullable: false),
                    KeyActivityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Minutes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskKeyActivity", x => x.PlanTaskId);
                    table.ForeignKey(
                        name: "FK_TaskKeyActivity_PlanTask_PlanTaskId",
                        column: x => x.PlanTaskId,
                        principalTable: "PlanTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskMedicationIntake",
                columns: table => new
                {
                    PlanTaskId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MedicationDoseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UnitsTotal = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitsTaken = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskMedicationIntake", x => x.PlanTaskId);
                    table.ForeignKey(
                        name: "FK_TaskMedicationIntake_PlanTask_PlanTaskId",
                        column: x => x.PlanTaskId,
                        principalTable: "PlanTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskTraining",
                columns: table => new
                {
                    PlanTaskId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TrainingType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTraining", x => x.PlanTaskId);
                    table.ForeignKey(
                        name: "FK_TaskTraining_PlanTask_PlanTaskId",
                        column: x => x.PlanTaskId,
                        principalTable: "PlanTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskTreadmill",
                columns: table => new
                {
                    PlanTaskId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Km = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTreadmill", x => x.PlanTaskId);
                    table.ForeignKey(
                        name: "FK_TaskTreadmill_PlanTask_PlanTaskId",
                        column: x => x.PlanTaskId,
                        principalTable: "PlanTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayTemplate_Id_Version",
                table: "DayTemplate",
                columns: new[] { "Id", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_DayTemplateItem_TemplateId_DisplayOrder",
                table: "DayTemplateItem",
                columns: new[] { "TemplateId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_KeyActivity_Name",
                table: "KeyActivity",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plan_StartDate_EndDate",
                table: "Plan",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanDay_PlanId_Date",
                table: "PlanDay",
                columns: new[] { "PlanId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanTask_PlanDayId_DisplayOrder",
                table: "PlanTask",
                columns: new[] { "PlanDayId", "DisplayOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DayTemplateItem");

            migrationBuilder.DropTable(
                name: "KeyActivity");

            migrationBuilder.DropTable(
                name: "MedicationDose");

            migrationBuilder.DropTable(
                name: "Plan");

            migrationBuilder.DropTable(
                name: "TaskKeyActivity");

            migrationBuilder.DropTable(
                name: "TaskMedicationIntake");

            migrationBuilder.DropTable(
                name: "TaskTraining");

            migrationBuilder.DropTable(
                name: "TaskTreadmill");

            migrationBuilder.DropTable(
                name: "DayTemplate");

            migrationBuilder.DropTable(
                name: "PlanTask");

            migrationBuilder.DropTable(
                name: "PlanDay");
        }
    }
}
