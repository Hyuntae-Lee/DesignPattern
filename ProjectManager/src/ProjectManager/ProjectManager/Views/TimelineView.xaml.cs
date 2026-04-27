using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectManager.Views
{
    /// <summary>
    /// Interaction logic for TimelineView.xaml
    /// </summary>
    public partial class TimelineView : System.Windows.Controls.UserControl
    {
        private DateTime _start;
        private int _days;
        private double _pxPerDay = 20.0;
        private double _rowHeight = 60.0;

        public TimelineView()
        {
            InitializeComponent();
            _start = GetMonday(DateTime.Today.AddDays(-30));
            _days = 365;
            Loaded += TimelineView_Loaded;
            Controllers.Controller.Instance.Projects.CollectionChanged += (_, __) => Render();
        }

        private void TimelineView_Loaded(object sender, RoutedEventArgs e) => Render();

        private double DateToX(DateTime d) => (d - _start).TotalDays * _pxPerDay;

        private DateTime GetMonday(DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7;
            return date.AddDays(-diff);
        }
        private double GetRangeOfMonth(DateTime date)
        {
            var first = new DateTime(date.Year, date.Month, 1);
            var nextMonth = first.AddMonths(1);
            return (nextMonth - first).TotalDays;
        }

        private void DrawMonths(double y)
        {
            var currentlyDrawingTime = _start;
            for (int i = 0; i <= _days; i++)
            {
                var thisTime = _start.AddDays(i);

                if (currentlyDrawingTime.Month == thisTime.Month) continue;

                currentlyDrawingTime = thisTime;

                var x = DateToX(thisTime);
                var widthOfMonth = GetRangeOfMonth(thisTime) * _pxPerDay;

                const double canvasHeight = 30.0;

                var tb = new TextBlock
                {
                    Text = currentlyDrawingTime.ToString("MMMM"),
                    Foreground = Brushes.Black,
                    Background = Brushes.Yellow,
                    Width = widthOfMonth,
                    Height = canvasHeight,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                Canvas.SetLeft(tb, x);
                Canvas.SetTop(tb, y);
                MainCanvas.Children.Add(tb);

                var lineBetweenMonths = new Line
                {
                    X1 = x,
                    X2 = x,
                    Y1 = 0,
                    Y2 = canvasHeight,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                MainCanvas.Children.Add(lineBetweenMonths);
            }
        }

        private void DrawWeeks(double y)
        {
            const double canvasHeight = 30.0;

            for (int i = 0; i <= _days; i++)
            {
                var thisTime = _start.AddDays(i);
                var x = DateToX(thisTime);

                var tb = new TextBlock
                {
                    Text = thisTime.ToString("dd"),
                    Foreground = Brushes.Black,
                    Background = Brushes.White,
                    Width = _pxPerDay,
                    Height = canvasHeight,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                Canvas.SetLeft(tb, x);
                Canvas.SetTop(tb, y);
                MainCanvas.Children.Add(tb);
            }
        }

        private void DrawTimeLines(double y)
        {
            for (int i = 0; i <= _days; i++)
            {
                var thisTime = _start.AddDays(i);
                var x = DateToX(thisTime);

                var line = new Line
                {
                    X1 = x,
                    Y1 = y,
                    X2 = x,
                    Y2 = MainCanvas.Height,
                    Stroke = (i % 7 == 0 ? Brushes.LightGray : Brushes.LightGray),
                    StrokeThickness = (i % 7 == 0 ? 1.0 : 0.5)
                };
                MainCanvas.Children.Add(line);
            }
        }

        private void Render()
        {
            MainCanvas.Children.Clear();
            var width = _days * _pxPerDay;
            MainCanvas.Width = Math.Max(800, width);

            // TimeLineLayer
            DrawMonths(0);
            DrawWeeks(30);
            DrawTimeLines(30);

            // Project and Task layers
            var projects = Controllers.Controller.Instance.Projects.ToList();
            for (int pi = 0; pi < projects.Count; pi++)
            {
                var p = projects[pi];
                double y = 60 + pi * _rowHeight;

                // Project header band
                var rect = new Rectangle { Width = MainCanvas.Width, Height = 24, Fill = p.Brush, Stroke = Brushes.Black, Opacity = 0.5 };
                Canvas.SetLeft(rect, 0);
                Canvas.SetTop(rect, y);
                MainCanvas.Children.Add(rect);

                var name = new TextBlock { Text = p.Name, FontWeight = FontWeights.Bold };
                Canvas.SetLeft(name, 6);
                Canvas.SetTop(name, y + 2);
                MainCanvas.Children.Add(name);

                // Tasks
                foreach (var t in p.Tasks)
                {
                    var x1 = DateToX(t.BeginDate);
                    var x2 = DateToX(t.EndDate);
                    var w = Math.Max(6, x2 - x1);
                    var taskRect = new Rectangle
                    {
                        Width = w,
                        Height = 28,
                        Fill = t.Brush,
                        Stroke = Brushes.Black,
                        RadiusX = 4,
                        RadiusY = 4
                    };
                    Canvas.SetLeft(taskRect, x1);
                    Canvas.SetTop(taskRect, y + 30);
                    MainCanvas.Children.Add(taskRect);

                    var tname = new TextBlock { Text = (t.Resource?.Name ?? "Task"), Foreground = Brushes.Black };
                    Canvas.SetLeft(tname, x1 + 4);
                    Canvas.SetTop(tname, y + 34);
                    MainCanvas.Children.Add(tname);
                }
            }
        }
    }
}
