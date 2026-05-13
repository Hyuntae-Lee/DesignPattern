using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.ViewModels
{
    public class TimeLineViewViewModel : INotifyPropertyChanged
    {
        private readonly Settings _settingsModel;

        public DateTime BeginTime
        {
            set
            {
                _settingsModel.BeginTime = value;
                OnPropertyChanged("BeginTime");
            }
            get => _settingsModel.BeginTime;
        }
        public DateTime EndTime
        {
            set
            {
                _settingsModel.EndTime = value;
                OnPropertyChanged("EndTime");
            }
            get => _settingsModel.EndTime;
        }

        public TimeLineViewViewModel(Settings settingsModel)
        {
            _settingsModel = settingsModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
