namespace LockIn.Abstractions;

public interface IAppReadOnlyState
{
    bool ReadOnlyMode { get; }
    void SetReadOnly(bool value);
}