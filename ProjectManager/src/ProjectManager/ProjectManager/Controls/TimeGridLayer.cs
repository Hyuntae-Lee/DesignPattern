using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace ProjectManager.Controls
{
    public class TimeGridLayer : FrameworkElement
    {
        private const double CellWidth = 20.0;

        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register(
                nameof(BeginTime),
                typeof(DateTime),
                typeof(TimeGridLayer),
                new FrameworkPropertyMetadata(
                    DateTime.Today,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register(
                nameof(EndTime),
                typeof(DateTime),
                typeof(TimeGridLayer),
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

            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

            int totalDays = GetTotalDays();
            if (totalDays == 0 || width <= 0 || height <= 0)
                return;

            var gridPen = new Pen(new SolidColorBrush(Color.FromRgb(220, 220, 220)), 1.0);
            gridPen.Freeze();

            var weekendBrush = new SolidColorBrush(Color.FromArgb(30, 200, 200, 255));
            weekendBrush.Freeze();

            double dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            var typeface = new Typeface("Segoe UI");

            for (int i = 0; i < totalDays; i++)
            {
                DateTime date = BeginTime.Date.AddDays(i);
                double x = i * CellWidth;

                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    dc.DrawRectangle(weekendBrush, null, new Rect(x, 0, CellWidth, height));

                dc.DrawLine(gridPen, new Point(x, 0), new Point(x, height));

                var label = new FormattedText(
                    date.Day.ToString(),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    10,
                    Brushes.DimGray,
                    dpi);

                dc.DrawText(label, new Point(x + (CellWidth - label.Width) / 2, 2));
            }

            dc.DrawLine(gridPen, new Point(totalDays * CellWidth, 0), new Point(totalDays * CellWidth, height));

            var borderPen = new Pen(new SolidColorBrush(Color.FromRgb(180, 180, 180)), 1.0);
            borderPen.Freeze();
            dc.DrawRectangle(null, borderPen, new Rect(0, 0, width, height));
        }
    }
}
