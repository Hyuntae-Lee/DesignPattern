using ProjectManager.Controllers;
using ProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectManager.Views
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    public partial class ProjectView : System.Windows.Controls.UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
            ProjectsList.ItemsSource = Controller.Instance.Projects;
            ProjectsList.SelectionChanged += ProjectsList_SelectionChanged;
            AddProjectBtn.Click += AddProjectBtn_Click;
            RemoveProjectBtn.Click += RemoveProjectBtn_Click;
            PickColorBtn.Click += PickColorBtn_Click;
            ProjectNameBox.LostFocus += ProjectNameBox_LostFocus;
        }

        private void ProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var p = ProjectsList.SelectedItem as Project;
            if (p == null)
            {
                ProjectNameBox.Text = "";
                ProjectColorRect.Fill = System.Windows.Media.Brushes.Transparent;
                return;
            }
            ProjectNameBox.Text = p.Name;
            ProjectColorRect.Fill = p.Brush;
        }

        private void AddProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            var p = new Project { Name = "New Project" };
            Controller.Instance.AddProject(p);
            ProjectsList.SelectedItem = p;
        }

        private void RemoveProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsList.SelectedItem is Project p)
                Controller.Instance.RemoveProject(p);
        }

        private void PickColorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsList.SelectedItem is Project p)
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var c = System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    p.Color = c;
                    ProjectColorRect.Fill = p.Brush;
                }
            }
        }

        private void ProjectNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ProjectsList.SelectedItem is Project p)
                p.Name = ProjectNameBox.Text;
        }
    }
}
