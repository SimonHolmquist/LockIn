// File: src/LockIn.Domain/Enums/Enums.cs
namespace LockIn.Domain.Enums;

public enum TaskType
{
    Desayuno, Almuerzo, Merienda, Cena, UltimaComida,
    Pastillas, Entrenamiento, Cinta, ActividadClave, Despertar, Dormir
}

public enum TrainingType { Push, Pull, Legs, FullBody, UpperBody, LowerBody }

public enum TaskStatus { Rojo, Amarillo, Verde }

public enum TemplateStatus { Borrador, Confirmada }

public enum PlanStatus { Borrador, Confirmado }
