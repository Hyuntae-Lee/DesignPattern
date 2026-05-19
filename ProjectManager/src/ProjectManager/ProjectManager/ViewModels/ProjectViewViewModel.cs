using ProjectManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ProjectManager.ViewModels
{
    public class ProjectViewViewModel : INotifyPropertyChanged
    {
        private readonly ProjectStorage _storage;
        private object _selectedItem;

        public ObservableCollection<Project> Projects => _storage.Projects;

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(SelectedProject));
                OnPropertyChanged(nameof(SelectedTask));
                OnPropertyChanged(nameof(IsProjectSelected));
                OnPropertyChanged(nameof(IsTaskSelected));
            }
        }

        public Project SelectedProject => _selectedItem as Project;
        public ProjectTask SelectedTask => _selectedItem as ProjectTask;
        public bool IsProjectSelected => _selectedItem is Project;
        public bool IsTaskSelected => _selectedItem is ProjectTask;

        public ICommand AddProjectCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand RemoveCommand { get; }

        public ProjectViewViewModel(ProjectStorage storage)
        {
            _storage = storage;

            AddProjectCommand = new RelayCommand(_ => AddProject());
            AddTaskCommand = new RelayCommand(_ => AddTask(), _ => GetOwningProject() != null);
            RemoveCommand = new RelayCommand(_ => Remove(), _ => _selectedItem != null);
        }

        private void AddProject()
        {
            var project = new Project { Name = "New Project " + (_storage.Projects.Count + 1) };
            _storage.Projects.Add(project);
            SelectedItem = project;
        }

        private void AddTask()
        {
            var owner = GetOwningProject();
            if (owner == null) return;

            var task = new ProjectTask { Name = "New Task " + (owner.ProjectTasks.Count + 1) };
            owner.ProjectTasks.Add(task);
            SelectedItem = task;
        }

        private void Remove()
        {
            if (SelectedTask != null)
            {
                foreach (var project in _storage.Projects)
                {
                    if (project.ProjectTasks.Remove(SelectedTask))
                    {
                        SelectedItem = project;
                        return;
                    }
                }
            }
            else if (SelectedProject != null)
            {
                _storage.Projects.Remove(SelectedProject);
                SelectedItem = null;
            }
        }

        private Project GetOwningProject()
        {
            if (SelectedProject != null)
                return SelectedProject;

            if (SelectedTask != null)
            {
                foreach (var p in _storage.Projects)
                    if (p.ProjectTasks.Contains(SelectedTask))
                        return p;
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
