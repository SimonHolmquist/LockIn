// File: src/LockIn.Domain/Common/DomainException.cs
namespace LockIn.Domain.Common;

using System;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

// Especificas
public sealed class TemplateAlreadyConfirmedException : DomainException
{
    public TemplateAlreadyConfirmedException() : base("La plantilla ya fue confirmada y es inmutable.") { }
}

public sealed class TemplateWithAssignmentsCannotBeDeletedException : DomainException
{
    public TemplateWithAssignmentsCannotBeDeletedException() : base("La plantilla no puede eliminarse porque posee asignaciones a planes.") { }
}

public sealed class InvalidManualOrderException : DomainException
{
    public InvalidManualOrderException() : base("OrdenManual invalido (debe ser >= 0).") { }
}

public sealed class OverlappingPlanRangeException : DomainException
{
    public OverlappingPlanRangeException() : base("La seleccion contiene dias ya planificados (no se permite solapamiento).") { }
}

public sealed class KeyActivityNotEditableException : DomainException
{
    public KeyActivityNotEditableException() : base("La Actividad Clave no es editable (solo alta/eliminacion).") { }
}

public sealed class MedicationInvalidDoseException : DomainException
{
    public MedicationInvalidDoseException(string? msg = null) : base(msg ?? "Dosis de medicamento invalida.") { }
}

public sealed class PlanAlreadyConfirmedException : DomainException
{
    public PlanAlreadyConfirmedException() : base("El plan ya fue confirmado y es inmutable.") { }
}

public sealed class PlannedDayLockedException : DomainException
{
    public PlannedDayLockedException() : base("El dia ya posee un plan confirmado (bloqueado).") { }
}
