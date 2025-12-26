using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using HostMonitor.Messages;
using HostMonitor.ViewModels;
using HostMonitor.Views;
using MaterialDesignThemes.Wpf;

namespace HostMonitor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        WeakReferenceMessenger.Default.Register<OpenAddEditDialogMessage>(this, async (_, message) =>
        {
            await ShowAddEditDialogAsync(message.ViewModel);
        });

        WeakReferenceMessenger.Default.Register<ConfirmDeleteHostMessage>(this, async (_, message) =>
        {
            await ShowDeleteConfirmAsync(message);
        });
    }

    /// <inheritdoc />
    protected override void OnClosed(EventArgs e)
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        base.OnClosed(e);
    }

    private static Task ShowAddEditDialogAsync(AddEditHostViewModel viewModel)
    {
        var dialog = new AddEditHostDialog
        {
            DataContext = viewModel
        };

        return DialogHost.Show(dialog, "RootDialog");
    }

    private static async Task ShowDeleteConfirmAsync(ConfirmDeleteHostMessage message)
    {
        var dialog = new ConfirmDeleteDialog
        {
            DataContext = message.Host
        };

        var result = await DialogHost.Show(dialog, "RootDialog");
        message.Completion.TrySetResult(result is bool confirmed && confirmed);
    }
}
