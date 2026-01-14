using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfPoint = System.Windows.Point;
using WpfColor = System.Windows.Media.Color;

namespace HostMonitor.Controls;

/// <summary>
/// A simple chart control for displaying response time history using WPF primitives.
/// </summary>
public class ResponseTimeChart : System.Windows.Controls.UserControl
{
    /// <summary>
    /// Identifies the <see cref="Values"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
        nameof(Values),
        typeof(ObservableCollection<double>),
        typeof(ResponseTimeChart),
        new PropertyMetadata(null, OnValuesChanged));

    /// <summary>
    /// Gets or sets the response time values.
    /// </summary>
    public ObservableCollection<double>? Values
    {
        get => (ObservableCollection<double>?)GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    private readonly Canvas _canvas;
    private readonly Polyline _line;
    private readonly Polygon _fill;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseTimeChart"/> class.
    /// </summary>
    public ResponseTimeChart()
    {
        _canvas = new Canvas
        {
            ClipToBounds = true
        };

        _fill = new Polygon
        {
            Fill = new SolidColorBrush(WpfColor.FromArgb(50, 30, 144, 255)),
            Stroke = null
        };

        _line = new Polyline
        {
            Stroke = new SolidColorBrush(WpfColor.FromRgb(30, 144, 255)),
            StrokeThickness = 2,
            StrokeLineJoin = PenLineJoin.Round
        };

        _canvas.Children.Add(_fill);
        _canvas.Children.Add(_line);

        Content = _canvas;
        SizeChanged += OnSizeChanged;
    }

    private static void OnValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ResponseTimeChart chart)
        {
            return;
        }

        if (e.OldValue is INotifyCollectionChanged oldCollection)
        {
            oldCollection.CollectionChanged -= chart.OnCollectionChanged;
        }

        if (e.NewValue is INotifyCollectionChanged newCollection)
        {
            newCollection.CollectionChanged += chart.OnCollectionChanged;
        }

        chart.UpdateChart();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateChart();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateChart();
    }

    private void UpdateChart()
    {
        _line.Points.Clear();
        _fill.Points.Clear();

        var values = Values;
        if (values is null || values.Count == 0)
        {
            return;
        }

        var width = ActualWidth;
        var height = ActualHeight;

        if (width <= 0 || height <= 0)
        {
            return;
        }

        var maxValue = values.Max();
        var minValue = Math.Min(0, values.Min());

        if (maxValue <= minValue)
        {
            maxValue = minValue + 1;
        }

        var range = maxValue - minValue;
        var padding = 4.0;
        var chartHeight = height - padding * 2;
        var chartWidth = width - padding * 2;

        var points = new PointCollection();
        var fillPoints = new PointCollection();

        for (var i = 0; i < values.Count; i++)
        {
            var x = padding + (chartWidth * i / Math.Max(1, values.Count - 1));
            var normalizedValue = (values[i] - minValue) / range;
            var y = height - padding - (normalizedValue * chartHeight);

            points.Add(new WpfPoint(x, y));
            fillPoints.Add(new WpfPoint(x, y));
        }

        // Add bottom corners for the fill polygon
        if (fillPoints.Count > 0)
        {
            fillPoints.Add(new WpfPoint(padding + chartWidth * (values.Count - 1) / Math.Max(1, values.Count - 1), height - padding));
            fillPoints.Add(new WpfPoint(padding, height - padding));
        }

        _line.Points = points;
        _fill.Points = fillPoints;
    }
}
