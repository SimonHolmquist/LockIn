namespace LockIn.Abstractions;

// File: src/LockIn.Application/Abstractions/IDateTimeProvider.cs
public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateOnly Today { get; }
}

