namespace LockIn.Abstractions;
public interface ISingleInstanceService : IDisposable
{
    /// Intenta adquirir la instancia primaria (mutex). true = somos la primera.
    bool TryAcquirePrimaryInstance();
    /// Inicia el receptor IPC para "ShowMainWindow" (solo primaria).
    void StartListeningForActivationRequests(Action onShowMainWindow);
    /// Señal a la instancia primaria para mostrar la ventana principal.
    void SignalFirstInstanceToShow();
}
