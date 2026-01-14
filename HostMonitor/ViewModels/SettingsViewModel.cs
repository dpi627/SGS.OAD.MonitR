using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HostMonitor.Messages;
using HostMonitor.Services;

namespace HostMonitor.ViewModels;

/// <summary>
/// View model for the settings dialog.
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly NotificationService _notificationService;

    [ObservableProperty]
    private int monitorIntervalSeconds;

    /// <summary>
    /// Gets the minimum interval in seconds.
    /// </summary>
    public int MinInterval => SettingsService.MinInterval;

    /// <summary>
    /// Gets the maximum interval in seconds.
    /// </summary>
    public int MaxInterval => SettingsService.MaxInterval;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    public SettingsViewModel(SettingsService settingsService, NotificationService notificationService)
    {
        _settingsService = settingsService;
        _notificationService = notificationService;
        MonitorIntervalSeconds = settingsService.MonitorIntervalSeconds;
    }

    /// <summary>
    /// Loads current settings.
    /// </summary>
    public void Load()
    {
        MonitorIntervalSeconds = _settingsService.MonitorIntervalSeconds;
    }

    [RelayCommand]
    private void Save()
    {
        if (!_settingsService.TrySetInterval(MonitorIntervalSeconds))
        {
            _notificationService.ShowWarning($"監控間隔需介於 {MinInterval} 到 {MaxInterval} 秒");
            return;
        }

        _notificationService.ShowSuccess("設定已儲存");
        WeakReferenceMessenger.Default.Send(new CloseDialogMessage("RootDialog"));
    }

    [RelayCommand]
    private void Cancel()
    {
        MonitorIntervalSeconds = _settingsService.MonitorIntervalSeconds;
        WeakReferenceMessenger.Default.Send(new CloseDialogMessage("RootDialog"));
    }
}
