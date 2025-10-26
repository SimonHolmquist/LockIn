// src/LockIn.Infrastructure/Paths/AppPaths.cs
using System;
using System.IO;

namespace LockIn.Infrastructure.Paths;
public static class AppPaths
{
    public static string AppDataRoot =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LockIn");

    public static string LogsDir => Path.Combine(AppPaths.AppDataRoot, "logs");

    public static string PrefsFile => Path.Combine(AppPaths.AppDataRoot, "prefs.json");

    public static void EnsureBaseFolders()
    {
        Directory.CreateDirectory(AppDataRoot);
        Directory.CreateDirectory(LogsDir);
    }
}
