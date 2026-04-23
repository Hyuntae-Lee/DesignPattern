using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace ProjectMngr.Models
{
    class VTask : INotifyPropertyChanged
    {
        private DateTime _beginDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today.AddDays(7);
        private Color _color = Colors.LightBlue;
        private VResource? _resource;

        public DateTime BeginDate
        {
            get => _beginDate;
            set { _beginDate = value; OnPropertyChanged(); }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        public Color Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); OnPropertyChanged(nameof(Brush)); }
        }

        public VResource? Resource
        {
            get => _resource;
            set { _resource = value; OnPropertyChanged(); }
        }

        public SolidColorBrush Brush => new SolidColorBrush(_color);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
