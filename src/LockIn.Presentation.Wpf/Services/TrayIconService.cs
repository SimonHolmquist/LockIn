// src/LockIn.Presentation.Wpf/Services/TrayIconService.cs
using System;
using System.Windows.Forms;

namespace LockIn.Presentation.Wpf.Services;
public sealed class TrayIconService : IDisposable
{
    private NotifyIcon? _ni;
    private Action? _onOpen;
    private Action? _onToggle;
    private Action? _onExit;

    public void Initialize(Action onOpen, Action onToggle, Action onExit)
    {
        _onOpen = onOpen;
        _onToggle = onToggle;
        _onExit = onExit;

        _ni = new NotifyIcon
        {
            Icon = System.Drawing.SystemIcons.Application,
            Text = "LockIn",
            Visible = true
        };

        var menu = new ContextMenuStrip();
        var abrir = new ToolStripMenuItem("Abrir Mi Día", null, (_, __) => _onOpen?.Invoke()) { Enabled = true };
        var planificador = new ToolStripMenuItem("Planificador") { Enabled = false };
        var plantillas = new ToolStripMenuItem("Plantillas") { Enabled = false };
        var metricas = new ToolStripMenuItem("Métricas") { Enabled = false };
        var ayuda = new ToolStripMenuItem("Ayuda") { Enabled = false };
        var salir = new ToolStripMenuItem("Salir", null, (_, __) => _onExit?.Invoke()) { Enabled = true };

        menu.Items.AddRange(new ToolStripItem[] { abrir, planificador, plantillas, metricas, ayuda, new ToolStripSeparator(), salir });

        _ni.ContextMenuStrip = menu;
        _ni.DoubleClick += (_, __) => _onToggle?.Invoke();
    }

    public void Dispose()
    {
        if (_ni is not null)
        {
            _ni.Visible = false;
            _ni.Dispose();
        }
    }
}
