// src/LockIn.Presentation.Wpf/App.xaml.cs
using CommunityToolkit.Mvvm.DependencyInjection;
using LockIn.Abstractions;
using LockIn.Infrastructure.Logging;
using LockIn.Infrastructure.Paths;
using LockIn.Infrastructure.Preferences;
using LockIn.Infrastructure.SingleInstance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace LockIn.Presentation.Wpf;
public partial class App : System.Windows.Application
{
    private IHost? _host;
    private ILogger<App>? _log;
    private ISingleInstanceService? _single;
    private Services.TrayIconService? _tray;
    private Window? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Cultura es-AR + 24h
        var culture = new CultureInfo("es-AR");
        culture.DateTimeFormat.ShortTimePattern = "HH:mm";
        culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        // Logging a archivo
        AppPaths.EnsureBaseFolders();

        _host = Host.CreateDefaultBuilder(e.Args)
            .ConfigureServices(services =>
            {
                // Infra
                services.AddSingleton<ILocalPrefs, LocalPrefsService>();
                services.AddSingleton<ISingleInstanceService, SingleInstanceService>();

                // MediatR (preparado, sin Handlers aún)
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(App).Assembly));

                // Tray
                services.AddSingleton<Services.TrayIconService>();

                // Vistas + VMs
                services.AddSingleton<ViewModels.MainWindowViewModel>();
                services.AddSingleton<Views.MainWindow>();
            })
            .ConfigureLogging(lb =>
            {
                lb.ClearProviders(); // sin consola/eventlog; offline-local
                lb.AddProvider(new FileLoggerProvider(AppPaths.LogsDir));
                lb.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        Ioc.Default.ConfigureServices(_host.Services);

        _log = _host.Services.GetRequiredService<ILogger<App>>();
        _single = _host.Services.GetRequiredService<ISingleInstanceService>();
        _tray = _host.Services.GetRequiredService<Services.TrayIconService>();

        // Manejo global de excepciones -> a log local
        this.DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

        // Instancia única
        if (!_single.TryAcquirePrimaryInstance())
        {
            _single.SignalFirstInstanceToShow();
            Shutdown(); // segunda instancia termina
            return;
        }

        _single.StartListeningForActivationRequests(ShowMainWindow);

        // Tray icon + menú
        _tray.Initialize(
            onOpen: ShowMainWindow,
            onToggle: ToggleMainWindowVisibility,
            onExit: () => { _log?.LogInformation("Saliendo desde menú de bandeja."); Shutdown(); }
        );

        // Shell oculta: no hay StartupUri; la ventana se crea on-demand
        _log.LogInformation("LockIn iniciado residente en tray.");
        base.OnStartup(e);
    }

    private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try { _log?.LogError(e.ExceptionObject as Exception, "Excepción no controlada (AppDomain)."); }
        catch { /* swallow */ }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try { _log?.LogError(e.Exception, "Excepción no controlada (Dispatcher)."); }
        catch { /* swallow */ }
        e.Handled = true; // evita caída dura
    }

    // reemplazá tu ShowMainWindow por este
    private void ShowMainWindow()
    {
        Dispatcher.Invoke(() =>
        {
            if (_mainWindow is null)
            {
                _mainWindow = _host!.Services.GetRequiredService<Views.MainWindow>();
            }

            // Siempre forzar estado Normal ANTES de mostrar
            _mainWindow.WindowState = WindowState.Normal;

            if (!_mainWindow.IsVisible)
                _mainWindow.Show();

            _mainWindow.Activate();
            _mainWindow.Focus();

            // Win32: restaurar/traer al frente
            WindowActivator.BringToFront(_mainWindow);

            // Nudge por si el shell no la trae al frente
            _mainWindow.Topmost = true;
            _mainWindow.Topmost = false;
        });
    }

    private void ToggleMainWindowVisibility()
    {
        Dispatcher.Invoke(() =>
        {
            if (_mainWindow is null || !_mainWindow.IsVisible)
            {
                ShowMainWindow();
                return;
            }

            // Si está minimizada, restaurar (no ocultar)
            if (_mainWindow.WindowState == WindowState.Minimized)
            {
                ShowMainWindow();
                return;
            }

            if (_mainWindow.Visibility == Visibility.Visible)
                _mainWindow.Hide();
            else
                ShowMainWindow();
        });
    }
    protected override void OnExit(ExitEventArgs e)
    {
        try { _tray?.Dispose(); _single?.Dispose(); }
        finally { _host?.Dispose(); }
        base.OnExit(e);
    }

    // dentro de la clase App (puede ir al final del archivo)
    private static class WindowActivator
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_RESTORE = 9;

        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void BringToFront(Window w)
        {
            var handle = new WindowInteropHelper(w).Handle;
            if (handle == IntPtr.Zero) return;

            ShowWindow(handle, SW_SHOWNORMAL);
            ShowWindow(handle, SW_RESTORE);
            SetForegroundWindow(handle);
        }
    }
}
