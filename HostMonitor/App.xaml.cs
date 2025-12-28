using System.ComponentModel;
using System.Drawing;
using System.Windows;
using HostMonitor.Services;
using HostMonitor.Services.Interfaces;
using HostMonitor.Services.Monitoring;
using HostMonitor.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using WinForms = System.Windows.Forms;

namespace HostMonitor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;
    private WinForms.NotifyIcon? _notifyIcon;
    private bool _isExiting;

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        services.AddSingleton<IHostDataService, HostDataService>();
        services.AddSingleton<MonitorOrchestrator>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<ThemeService>();
        services.AddTransient<IMonitorService, PingMonitorService>();
        services.AddTransient<IMonitorService, TcpPortMonitorService>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<HostListViewModel>();
        services.AddTransient<AddEditHostViewModel>();

        services.AddTransient<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        _serviceProvider.GetRequiredService<ThemeService>()
            .InitializeWithSystemTheme();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.StateChanged += OnMainWindowStateChanged;
        mainWindow.Closing += OnMainWindowClosing;

        InitializeNotifyIcon(mainWindow);

        mainWindow.Show();
    }

    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        _notifyIcon?.Dispose();
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private void InitializeNotifyIcon(MainWindow mainWindow)
    {
        var notificationService = _serviceProvider!.GetRequiredService<NotificationService>();

        _notifyIcon = new WinForms.NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "HostMonitor",
            Visible = true,
            ContextMenuStrip = BuildContextMenu(mainWindow)
        };

        _notifyIcon.DoubleClick += (_, _) => ToggleMainWindow(mainWindow);
        notificationService.NotificationRaised += (_, args) => ShowBalloonTip(args);
    }

    private WinForms.ContextMenuStrip BuildContextMenu(MainWindow mainWindow)
    {
        var menu = new WinForms.ContextMenuStrip();
        var showHide = new WinForms.ToolStripMenuItem("顯示/隱藏", null, (_, _) => ToggleMainWindow(mainWindow));
        var start = new WinForms.ToolStripMenuItem("開始監控", null, (_, _) => ExecuteStartMonitoring(mainWindow));
        var stop = new WinForms.ToolStripMenuItem("停止監控", null, (_, _) => ExecuteStopMonitoring(mainWindow));
        var exit = new WinForms.ToolStripMenuItem("結束", null, (_, _) => ExitApplication());

        menu.Items.Add(showHide);
        menu.Items.Add(start);
        menu.Items.Add(stop);
        menu.Items.Add(new WinForms.ToolStripSeparator());
        menu.Items.Add(exit);

        return menu;
    }

    private static void ExecuteStartMonitoring(MainWindow mainWindow)
    {
        if (mainWindow.DataContext is MainViewModel viewModel)
        {
            viewModel.StartMonitoringCommand.Execute(null);
        }
    }

    private static void ExecuteStopMonitoring(MainWindow mainWindow)
    {
        if (mainWindow.DataContext is MainViewModel viewModel)
        {
            viewModel.StopMonitoringCommand.Execute(null);
        }
    }

    private void ToggleMainWindow(MainWindow mainWindow)
    {
        if (mainWindow.IsVisible)
        {
            mainWindow.Hide();
            return;
        }

        mainWindow.Show();
        mainWindow.WindowState = WindowState.Normal;
        mainWindow.Activate();
    }

    private void OnMainWindowClosing(object? sender, CancelEventArgs e)
    {
        if (!_isExiting)
        {
            _isExiting = true;
        }
    }

    private static void OnMainWindowStateChanged(object? sender, EventArgs e)
    {
        if (sender is not Window window)
        {
            return;
        }

        if (window.WindowState == WindowState.Minimized)
        {
            window.Hide();
        }
    }

    private void ExitApplication()
    {
        _isExiting = true;
        Shutdown();
    }

    private void ShowBalloonTip(NotificationEventArgs args)
    {
        if (_notifyIcon is null)
        {
            return;
        }

        _notifyIcon.BalloonTipTitle = "HostMonitor";
        _notifyIcon.BalloonTipText = args.Message;
        _notifyIcon.BalloonTipIcon = args.Kind switch
        {
            NotificationKind.Warning => WinForms.ToolTipIcon.Warning,
            NotificationKind.Error => WinForms.ToolTipIcon.Error,
            _ => WinForms.ToolTipIcon.Info
        };
        _notifyIcon.ShowBalloonTip(3000);
    }
}
