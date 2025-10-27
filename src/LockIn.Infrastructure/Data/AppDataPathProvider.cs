using LockIn.Abstractions;

namespace LockIn.Infrastructure.Data;

internal sealed class AppDataPathProvider : IAppDataPathProvider
{
    private const string FolderName = "LockIn";
    private const string DbFileName = "lockin.db";

    public string GetDataDirectory()
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FolderName);

    public string GetDatabasePath()
        => Path.Combine(GetDataDirectory(), DbFileName);

    public void EnsureDataDirectory()
    {
        var dir = GetDataDirectory();
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }
}
