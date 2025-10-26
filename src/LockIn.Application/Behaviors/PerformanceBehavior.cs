// File: src/LockIn.Application/Behaviors/PerformanceBehavior.cs
namespace LockIn.Application.Behaviors;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try { return await next(); }
        finally { sw.Stop(); _logger.LogInformation("Handler {Handler} t={Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds); }
    }
}
