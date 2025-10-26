// src/LockIn.Infrastructure/Logging/FileLogger.cs
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace LockIn.Infrastructure.Logging;
internal sealed class FileLogger : ILogger
{
    private readonly string _category;
    private readonly Func<string> _currentFilePathProvider;
    private static readonly object _sync = new();

    public FileLogger(string category, Func<string> currentFilePathProvider)
    {
        _category = category;
        _currentFilePathProvider = currentFilePathProvider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_category} :: {formatter(state, exception)}";
        if (exception is not null) line += Environment.NewLine + exception;

        lock (_sync)
        {
            var path = _currentFilePathProvider();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
        }
    }
}
