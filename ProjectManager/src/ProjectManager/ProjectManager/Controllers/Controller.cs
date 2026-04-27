using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Controllers
{
    class Controller
    {
        private static Controller _instance;
        private Controller()
        {
            // seed data for demo
            var r1 = new Resource { Name = "Alice", Color = System.Windows.Media.Colors.PaleVioletRed };
            var r2 = new Resource { Name = "Bob", Color = System.Windows.Media.Colors.LightSeaGreen };
            Resources.Add(r1);
            Resources.Add(r2);

            var p1 = new Project { Name = "Project A", Color = System.Windows.Media.Colors.LightGreen };
            p1.Tasks.Add(
                new Models.Task
                {
                    BeginDate = System.DateTime.Today.AddDays(-10),
                    EndDate = System.DateTime.Today.AddDays(5),
                    Color = System.Windows.Media.Colors.LightBlue,
                    Resource = r1
                });
            p1.Tasks.Add(
                new Models.Task
                {
                    BeginDate = System.DateTime.Today.AddDays(10),
                    EndDate = System.DateTime.Today.AddDays(25),
                    Color = System.Windows.Media.Colors.Orange,
                    Resource = r2
                });
            Projects.Add(p1);

            var p2 = new Project { Name = "Project B", Color = System.Windows.Media.Colors.LightYellow };
            p2.Tasks.Add(new Models.Task { BeginDate = System.DateTime.Today.AddDays(0), EndDate = System.DateTime.Today.AddDays(14), Color = System.Windows.Media.Colors.LightCoral, Resource = r2 });
            Projects.Add(p2);
        }

        public static Controller Instance => _instance = new Controller();

        public ObservableCollection<Project> Projects { get; } = new ObservableCollection<Project>();
        public ObservableCollection<Resource> Resources { get; } = new ObservableCollection<Resource>();

        public void AddProject(Project project) => Projects.Add(project);
        public void RemoveProject(Project project) => Projects.Remove(project);

        public void AddResource(Resource resource) => Resources.Add(resource);
        public void RemoveResource(Resource resource) => Resources.Remove(resource);
    }
}
