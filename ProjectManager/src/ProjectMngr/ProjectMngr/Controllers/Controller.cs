using ProjectMngr.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ProjectMngr.Controllers
{
    class Controller
    {
        private static Controller? _instance;
        private Controller()
        {
            // seed data for demo
            var r1 = new VResource { Name = "Alice", Color = System.Windows.Media.Colors.PaleVioletRed };
            var r2 = new VResource { Name = "Bob", Color = System.Windows.Media.Colors.LightSeaGreen };
            Resources.Add(r1);
            Resources.Add(r2);

            var p1 = new VProject { Name = "Project A", Color = System.Windows.Media.Colors.LightGreen };
            p1.Tasks.Add(new VTask { BeginDate = System.DateTime.Today.AddDays(-10), EndDate = System.DateTime.Today.AddDays(5), Color = System.Windows.Media.Colors.LightBlue, Resource = r1 });
            p1.Tasks.Add(new VTask { BeginDate = System.DateTime.Today.AddDays(10), EndDate = System.DateTime.Today.AddDays(25), Color = System.Windows.Media.Colors.Orange, Resource = r2 });
            Projects.Add(p1);

            var p2 = new VProject { Name = "Project B", Color = System.Windows.Media.Colors.LightYellow };
            p2.Tasks.Add(new VTask { BeginDate = System.DateTime.Today.AddDays(0), EndDate = System.DateTime.Today.AddDays(14), Color = System.Windows.Media.Colors.LightCoral, Resource = r2 });
            Projects.Add(p2);
        }

        public static Controller Instance => _instance ??= new Controller();

        public ObservableCollection<VProject> Projects { get; } = new ObservableCollection<VProject>();
        public ObservableCollection<VResource> Resources { get; } = new ObservableCollection<VResource>();

        public void AddProject(VProject project) => Projects.Add(project);
        public void RemoveProject(VProject project) => Projects.Remove(project);

        public void AddResource(VResource resource) => Resources.Add(resource);
        public void RemoveResource(VResource resource) => Resources.Remove(resource);
    }
}
