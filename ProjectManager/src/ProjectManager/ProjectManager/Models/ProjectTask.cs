using System;
using System.ComponentModel;

namespace ProjectManager.Models
{
    public class ProjectTask : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _beginTime;
        private DateTime _endTime;
        private ProjectResource _assignee;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public DateTime BeginTime
        {
            get => _beginTime;
            set { _beginTime = value; OnPropertyChanged(nameof(BeginTime)); }
        }

        public DateTime EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(nameof(EndTime)); }
        }

        public ProjectResource Assignee
        {
            get => _assignee;
            set { _assignee = value; OnPropertyChanged(nameof(Assignee)); }
        }

        public ProjectTask()
        {
            _name = "New Task";
            _beginTime = DateTime.Today;
            _endTime = DateTime.Today.AddDays(1);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
