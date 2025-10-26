// src/LockIn.Presentation.Wpf/Views/MainWindow.xaml.cs
using System.ComponentModel;
using System.Windows;

namespace LockIn.Presentation.Wpf.Views;
public partial class MainWindow : Window
{
    public MainWindow(ViewModels.MainWindowViewModel vm)
    {
        InitializeComponent();
        Loaded += (_, __) => vm.Start();
        Closing += OnClosingHide; // mantener residente
    }

    private void OnClosingHide(object? sender, CancelEventArgs e)
    {
        // En F0 cerramos a bandeja; "Salir" se hace desde el menú
        e.Cancel = true;
        Hide();
    }
}
