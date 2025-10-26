// src/LockIn.Infrastructure/Preferences/LocalPrefsService.cs
using System.Text.Json;
using LockIn.Abstractions;
using LockIn.Infrastructure.Paths;

namespace LockIn.Infrastructure.Preferences;
public sealed class LocalPrefsService : ILocalPrefs
{
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true };

    public async Task<IDictionary<string, bool>> GetNoVolverAMostrarFlagsAsync(CancellationToken ct = default)
    {
        AppPaths.EnsureBaseFolders();
        if (!File.Exists(AppPaths.PrefsFile))
            return new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        await using var fs = File.OpenRead(AppPaths.PrefsFile);
        var data = await JsonSerializer.DeserializeAsync<PrefsDto>(fs, cancellationToken: ct) ?? new PrefsDto();
        return data.NoVolverAMostrar ?? new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
    }

    public async Task SetNoVolverAMostrarFlagAsync(string key, bool value, CancellationToken ct = default)
    {
        AppPaths.EnsureBaseFolders();
        PrefsDto dto;

        if (File.Exists(AppPaths.PrefsFile))
        {
            await using var fs = File.OpenRead(AppPaths.PrefsFile);
            dto = await JsonSerializer.DeserializeAsync<PrefsDto>(fs, cancellationToken: ct) ?? new PrefsDto();
        }
        else dto = new PrefsDto();

        dto.NoVolverAMostrar ??= new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        dto.NoVolverAMostrar[key] = value;

        await using var ofs = File.Create(AppPaths.PrefsFile);
        await JsonSerializer.SerializeAsync(ofs, dto, _json, ct);
    }

    private sealed class PrefsDto
    {
        public Dictionary<string, bool>? NoVolverAMostrar { get; set; }
    }
}
