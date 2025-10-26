// src/LockIn.Presentation.Wpf/ViewModels/MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Threading;

namespace LockIn.Presentation.Wpf.ViewModels;
public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime now = DateTime.Now;

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };

    public void Start()
    {
        _timer.Tick += (_, __) => Now = DateTime.Now;
        _timer.Start();
    }
}
