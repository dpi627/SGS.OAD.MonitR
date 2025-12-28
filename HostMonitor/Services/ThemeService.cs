using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace HostMonitor.Services;

/// <summary>
/// Provides theme switching and system theme detection.
/// </summary>
public sealed class ThemeService
{
    private readonly PaletteHelper _paletteHelper = new();

    /// <summary>
    /// Gets a value indicating whether the current theme is dark.
    /// </summary>
    public bool IsDarkTheme { get; private set; }

    /// <summary>
    /// Applies the current system theme to the application.
    /// </summary>
    public void InitializeWithSystemTheme()
    {
        SetTheme(IsSystemDarkTheme());
    }

    /// <summary>
    /// Applies a light or dark theme to the application.
    /// </summary>
    public void SetTheme(bool isDarkTheme)
    {
        if (IsDarkTheme == isDarkTheme)
        {
            return;
        }

        IsDarkTheme = isDarkTheme;

        var theme = _paletteHelper.GetTheme();
        theme.SetBaseTheme(isDarkTheme ? BaseTheme.Dark : BaseTheme.Light);
        _paletteHelper.SetTheme(theme);
    }

    private static bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            if (value is int intValue)
            {
                return intValue == 0;
            }
        }
        catch
        {
        }

        return false;
    }
}
