// src/LockIn.Infrastructure/SingleInstance/SingleInstanceService.cs
using System.IO.Pipes;
using System.Text;
using LockIn.Abstractions;

namespace LockIn.Infrastructure.SingleInstance;
public sealed class SingleInstanceService : ISingleInstanceService
{
    private const string MutexName = @"Global\LockIn.App";
    private const string PipeName = "LockInPipe";
    private Mutex? _mutex;
    private CancellationTokenSource? _cts;

    public bool TryAcquirePrimaryInstance()
    {
        _mutex = new Mutex(initiallyOwned: true, name: MutexName, out bool createdNew);
        if (!createdNew)
        {
            _mutex.Dispose();
            _mutex = null;
            return false;
        }
        return true;
    }

    public void StartListeningForActivationRequests(Action onShowMainWindow)
    {
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        _ = Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var server = new NamedPipeServerStream(PipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    await server.WaitForConnectionAsync(ct).ConfigureAwait(false);

                    using var ms = new MemoryStream();
                    await server.CopyToAsync(ms, ct).ConfigureAwait(false);
                    var msg = Encoding.UTF8.GetString(ms.ToArray()).Trim();

                    if (string.Equals(msg, "SHOW", StringComparison.OrdinalIgnoreCase))
                    {
                        onShowMainWindow?.Invoke();
                    }
                }
                catch (OperationCanceledException) { }
                catch { /* swallow: logging vendrá desde Presentation */ }
            }
        }, ct);
    }

    public void SignalFirstInstanceToShow()
    {
        try
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
            client.Connect(timeout: 800); // rápido y sin bloqueos
            var bytes = Encoding.UTF8.GetBytes("SHOW");
            client.Write(bytes, 0, bytes.Length);
            client.Flush();
        }
        catch { /* si no conecta, no hacemos nada más */ }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _mutex?.Dispose();
    }
}
