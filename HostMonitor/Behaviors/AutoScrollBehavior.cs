using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HostMonitor.Behaviors;

/// <summary>
/// Provides auto-scroll behavior for items controls.
/// </summary>
public static class AutoScrollBehavior
{
    /// <summary>
    /// Identifies the Enable attached property.
    /// </summary>
    public static readonly DependencyProperty EnableProperty =
        DependencyProperty.RegisterAttached(
            "Enable",
            typeof(bool),
            typeof(AutoScrollBehavior),
            new PropertyMetadata(false, OnEnableChanged));

    private static readonly DependencyProperty HandlerProperty =
        DependencyProperty.RegisterAttached(
            "Handler",
            typeof(NotifyCollectionChangedEventHandler),
            typeof(AutoScrollBehavior),
            new PropertyMetadata(null));

    /// <summary>
    /// Sets whether auto-scroll is enabled.
    /// </summary>
    public static void SetEnable(DependencyObject element, bool value)
    {
        element.SetValue(EnableProperty, value);
    }

    /// <summary>
    /// Gets whether auto-scroll is enabled.
    /// </summary>
    public static bool GetEnable(DependencyObject element)
    {
        return (bool)element.GetValue(EnableProperty);
    }

    private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ItemsControl itemsControl)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            itemsControl.Loaded += OnLoaded;
            itemsControl.Unloaded += OnUnloaded;
        }
        else
        {
            itemsControl.Loaded -= OnLoaded;
            itemsControl.Unloaded -= OnUnloaded;
            Detach(itemsControl);
        }
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is ItemsControl itemsControl)
        {
            Attach(itemsControl);
            ScrollToEnd(itemsControl);
        }
    }

    private static void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is ItemsControl itemsControl)
        {
            Detach(itemsControl);
        }
    }

    private static void Attach(ItemsControl itemsControl)
    {
        if (GetHandler(itemsControl) is not null)
        {
            return;
        }

        if (itemsControl.Items is INotifyCollectionChanged notify)
        {
            NotifyCollectionChangedEventHandler handler = (_, _) => ScrollToEnd(itemsControl);
            notify.CollectionChanged += handler;
            SetHandler(itemsControl, handler);
        }
    }

    private static void Detach(ItemsControl itemsControl)
    {
        if (itemsControl.Items is INotifyCollectionChanged notify)
        {
            var handler = GetHandler(itemsControl);
            if (handler is not null)
            {
                notify.CollectionChanged -= handler;
                SetHandler(itemsControl, null);
            }
        }
    }

    private static void ScrollToEnd(ItemsControl itemsControl)
    {
        var scrollViewer = FindScrollViewer(itemsControl);
        scrollViewer?.ScrollToEnd();
    }

    private static ScrollViewer? FindScrollViewer(DependencyObject root)
    {
        if (root is ScrollViewer viewer)
        {
            return viewer;
        }

        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            var result = FindScrollViewer(child);
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }

    private static NotifyCollectionChangedEventHandler? GetHandler(DependencyObject element)
    {
        return (NotifyCollectionChangedEventHandler?)element.GetValue(HandlerProperty);
    }

    private static void SetHandler(DependencyObject element, NotifyCollectionChangedEventHandler? handler)
    {
        element.SetValue(HandlerProperty, handler);
    }
}
