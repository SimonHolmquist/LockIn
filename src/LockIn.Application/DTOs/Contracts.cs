// File: src/LockIn.Application/DTOs/Contracts.cs
namespace LockIn.Application.DTOs;

using System;
using System.Collections.Generic;
using LockIn.Domain.Enums;

public sealed record BloqueDto(Guid Id, TimeOnly HoraInicio, int OrdenManual, TaskType Tipo);

public sealed record PlantillaDto(Guid Id, string Nombre, int Version, string Estado, IReadOnlyCollection<BloqueDto> Bloques);

public sealed record PlanTaskDto(Guid Id, DateOnly Fecha, TaskType Tipo, TimeOnly? HoraInicio, string Estado);

public sealed record PlanDto(Guid Id, DateOnly Fecha, string Estado, IReadOnlyCollection<PlanTaskDto> Tareas);

public sealed record PlanPreviewResultDto(IReadOnlyCollection<DateOnly> FechasSolicitadas,
                                          IReadOnlyCollection<DateOnly> FechasLibres,
                                          IReadOnlyCollection<DateOnly> FechasBloqueadas,
                                          bool TieneConflictos);

public enum DateSelectionMode { Rango, Multiseleccion, Patron }

public sealed record DateSelectionDto(DateSelectionMode Mode,
                                      DateOnly? Start,
                                      DateOnly? End,
                                      IReadOnlyCollection<DateOnly>? Dates,
                                      IReadOnlyCollection<DayOfWeek>? PatternDays);
