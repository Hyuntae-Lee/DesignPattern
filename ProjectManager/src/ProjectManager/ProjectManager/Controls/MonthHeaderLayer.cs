using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace ProjectManager.Controls
{
    public class MonthHeaderLayer : FrameworkElement
    {
        private const double CellWidth = 20.0;

        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register(
                nameof(BeginTime),
                typeof(DateTime),
                typeof(MonthHeaderLayer),
                new FrameworkPropertyMetadata(
                    DateTime.Today,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register(
                nameof(EndTime),
                typeof(DateTime),
                typeof(MonthHeaderLayer),
                new FrameworkPropertyMetadata(
                    DateTime.Today.AddDays(30),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public DateTime BeginTime
        {
            get => (DateTime)GetValue(BeginTimeProperty);
            set => SetValue(BeginTimeProperty, value);
        }

        public DateTime EndTime
        {
            get => (DateTime)GetValue(EndTimeProperty);
            set => SetValue(EndTimeProperty, value);
        }

        private int GetTotalDays()
        {
            if (EndTime.Date < BeginTime.Date)
                return 0;
            return (int)(EndTime.Date - BeginTime.Date).TotalDays + 1;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double desiredWidth = GetTotalDays() * CellWidth;
            double desiredHeight = double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height;
            return new Size(desiredWidth, desiredHeight);
        }

        protected override void OnRender(DrawingContext dc)
        {
            double width = ActualWidth;
            double height = ActualHeight;

            dc.DrawRectangle(Brushes.WhiteSmoke, null, new Rect(0, 0, width, height));

            int totalDays = GetTotalDays();
            if (totalDays == 0 || width <= 0 || height <= 0)
                return;

            var separatorPen = new Pen(new SolidColorBrush(Color.FromRgb(180, 180, 180)), 1.0);
            separatorPen.Freeze();

            double dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            var typeface = new Typeface("Segoe UI");

            int groupStart = 0;
            DateTime groupDate = BeginTime.Date;

            for (int i = 1; i <= totalDays; i++)
            {
                bool isLast = i == totalDays;
                DateTime current = BeginTime.Date.AddDays(i);
                bool monthChanged = !isLast &&
                                    (current.Year != groupDate.Year || current.Month != groupDate.Month);

                if (isLast || monthChanged)
                {
                    double x = groupStart * CellWidth;
                    double w = (i - groupStart) * CellWidth;

                    string label = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0:MMMM}, {1}",
                        groupDate,
                        groupDate.Year);

                    var text = new FormattedText(
                        label,
                        CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        12,
                        Brushes.Black,
                        dpi);

                    if (text.Width <= w - 4)
                    {
                        dc.DrawText(text, new Point(x + (w - text.Width) / 2, (height - text.Height) / 2));
                    }
                    else
                    {
                        dc.PushClip(new RectangleGeometry(new Rect(x + 2, 0, Math.Max(0, w - 4), height)));
                        dc.DrawText(text, new Point(x + 2, (height - text.Height) / 2));
                        dc.Pop();
                    }

                    dc.DrawLine(separatorPen, new Point(x, 0), new Point(x, height));

                    groupStart = i;
                    groupDate = current;
                }
            }

            dc.DrawLine(separatorPen, new Point(totalDays * CellWidth, 0), new Point(totalDays * CellWidth, height));

            var borderPen = new Pen(new SolidColorBrush(Color.FromRgb(180, 180, 180)), 1.0);
            borderPen.Freeze();
            dc.DrawRectangle(null, borderPen, new Rect(0, 0, width, height));
        }
    }
}
