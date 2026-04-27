using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectManager.Models
{
    class Project : INotifyPropertyChanged
    {
        private string _name = "Project";
        private System.Windows.Media.Color _color = Colors.LightGreen;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public System.Windows.Media.Color Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); OnPropertyChanged(nameof(Brush)); }
        }

        public ObservableCollection<Task> Tasks { get; } = new ObservableCollection<Task>();

        public Task GetTask(int index) => (index >= 0 && index < Tasks.Count) ? Tasks[index] : null;

        public SolidColorBrush Brush => new SolidColorBrush(_color);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
