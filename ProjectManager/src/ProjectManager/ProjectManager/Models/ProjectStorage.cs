using System.Collections.ObjectModel;

namespace ProjectManager.Models
{
    public class ProjectStorage
    {
        public ObservableCollection<Project> Projects { get; }

        public ProjectStorage()
        {
            Projects = new ObservableCollection<Project>();
        }
    }
}
