namespace LockIn.Abstractions;

public interface ILocalPrefs
{
    Task<IDictionary<string, bool>> GetNoVolverAMostrarFlagsAsync(CancellationToken ct = default);
    Task SetNoVolverAMostrarFlagAsync(string key, bool value, CancellationToken ct = default);
}
