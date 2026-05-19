using ProjectManager.Models;
using ProjectManager.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SqliteStorageService _sqliteStorageService;
        private readonly Settings _settings;
        private readonly ProjectStorage _projectStorage;
        private readonly ProjectResourceStorage _projectResourceStorage;

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public MainViewModel(SqliteStorageService sqliteStorageService,
            Settings settings, ProjectStorage projectStorage,
            ProjectResourceStorage projectResourceStorage)
        {
            _sqliteStorageService = sqliteStorageService;
            _settings = settings;
            _projectStorage = projectStorage;
            _projectResourceStorage = projectResourceStorage;

            SaveCommand = new RelayCommand(_ => SaveData());
            LoadCommand = new RelayCommand(_ => LoadData());
        }

        private bool SaveData()
        {
            _sqliteStorageService.SaveSettings();
            _sqliteStorageService.SaveResources();
            _sqliteStorageService.SaveProjects();

            return true;
        }

        private bool LoadData()
        {
            _sqliteStorageService.LoadSettings();
            _sqliteStorageService.LoadResources();
            _sqliteStorageService.LoadProjects();

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
