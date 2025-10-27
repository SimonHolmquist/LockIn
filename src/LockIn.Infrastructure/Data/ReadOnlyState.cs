using LockIn.Abstractions;

namespace LockIn.Infrastructure.Data;

internal sealed class ReadOnlyState : IAppReadOnlyState
{
    public bool ReadOnlyMode { get; private set; }
    public void SetReadOnly(bool value) => ReadOnlyMode = value;
}