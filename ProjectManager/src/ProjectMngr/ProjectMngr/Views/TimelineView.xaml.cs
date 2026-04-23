using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace ProjectMngr.Views
{
    /// <summary>
    /// Interaction logic for TimelineView.xaml
    /// </summary>
    public partial class TimelineView : System.Windows.Controls.UserControl
    {
        private DateTime _start;
        private int _days;
        private double _pxPerDay = 8.0;
        private double _rowHeight = 60.0;

        public TimelineView()
        {
            InitializeComponent();
            _start = DateTime.Today.AddDays(-30);
            _days = 180;
            Loaded += TimelineView_Loaded;
            Controllers.Controller.Instance.Projects.CollectionChanged += (_, __) => Render();
        }

        private void TimelineView_Loaded(object sender, RoutedEventArgs e) => Render();

        private double DateToX(DateTime d) => (d - _start).TotalDays * _pxPerDay;

        private void Render()
        {
            MainCanvas.Children.Clear();
            var width = _days * _pxPerDay;
            MainCanvas.Width = Math.Max(800, width + 200);

            // TimeLineLayer: draw week ticks and month labels
            for (int i = 0; i <= _days; i++)
            {
                var x = DateToX(_start.AddDays(i));
                var line = new Line { X1 = x, Y1 = 0, X2 = x, Y2 = MainCanvas.Height, Stroke = (i % 7 == 0 ? Brushes.LightGray : Brushes.LightGray), StrokeThickness = (i % 7 == 0 ? 1.0 : 0.5) };
                MainCanvas.Children.Add(line);
                if (i % 7 == 0)
                {
                    var dt = _start.AddDays(i);
                    var tb = new TextBlock { Text = dt.ToString("dd MMM"), Foreground = Brushes.Gray };
                    Canvas.SetLeft(tb, x + 2);
                    Canvas.SetTop(tb, 0);
                    MainCanvas.Children.Add(tb);
                }
            }

            // Project and Task layers
            var projects = Controllers.Controller.Instance.Projects.ToList();
            for (int pi = 0; pi < projects.Count; pi++)
            {
                var p = projects[pi];
                double y = 30 + pi * _rowHeight;

                // Project header band
                var rect = new Rectangle { Width = MainCanvas.Width - 20, Height = 24, Fill = p.Brush, Stroke = Brushes.Black, Opacity = 0.5 };
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
