using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ProjectManager.Models
{
    public class Project : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public ObservableCollection<ProjectTask> ProjectTasks { get; }

        public Project()
        {
            _name = "New Project";
            ProjectTasks = new ObservableCollection<ProjectTask>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
