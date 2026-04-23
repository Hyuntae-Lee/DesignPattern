using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace ProjectMngr.Models
{
    class VResource : INotifyPropertyChanged
    {
        private string _name = "Resource";
        private Color _color = Colors.LightGray;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public Color Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); OnPropertyChanged(nameof(Brush)); }
        }

        public SolidColorBrush Brush => new SolidColorBrush(_color);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
