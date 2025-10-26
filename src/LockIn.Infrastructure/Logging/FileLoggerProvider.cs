// src/LockIn.Infrastructure/Logging/FileLoggerProvider.cs
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace LockIn.Infrastructure.Logging;
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logsDir;
    public FileLoggerProvider(string logsDir)
    {
        _logsDir = logsDir;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, CurrentFile);
    }

    private string CurrentFile()
    {
        var file = $"lockin-{DateTime.Now:yyyyMMdd}.log";
        return Path.Combine(_logsDir, file);
    }

    public void Dispose() { }
}
