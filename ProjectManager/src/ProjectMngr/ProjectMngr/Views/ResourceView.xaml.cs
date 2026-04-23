using ProjectMngr.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Controller = ProjectMngr.Controllers.Controller;

namespace ProjectMngr.Views
{
    /// <summary>
    /// Interaction logic for ResourceView.xaml
    /// </summary>
    public partial class ResourceView : System.Windows.Controls.UserControl
    {
        public ResourceView()
        {
            InitializeComponent();
            ResourcesList.ItemsSource = Controller.Instance.Resources;
            ResourcesList.SelectionChanged += ResourcesList_SelectionChanged;
            AddResourceBtn.Click += AddResourceBtn_Click;
            RemoveResourceBtn.Click += RemoveResourceBtn_Click;
            PickColorBtn.Click += PickColorBtn_Click;
            ResourceNameBox.LostFocus += ResourceNameBox_LostFocus;
        }

        private void ResourcesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResourcesList.SelectedItem is VResource r)
            {
                ResourceNameBox.Text = r.Name;
                ResourceColorRect.Fill = r.Brush;
            }
            else
            {
                ResourceNameBox.Text = "";
                ResourceColorRect.Fill = System.Windows.Media.Brushes.Transparent;
            }
        }

        private void AddResourceBtn_Click(object sender, RoutedEventArgs e)
        {
            var r = new VResource { Name = "New Resource" };
            Controller.Instance.AddResource(r);
            ResourcesList.SelectedItem = r;
        }

        private void RemoveResourceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ResourcesList.SelectedItem is VResource r)
                Controller.Instance.RemoveResource(r);
        }

        private void PickColorBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ResourcesList.SelectedItem is VResource r)
            {
                var dlg = new ColorDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var c = System.Windows.Media.Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                    r.Color = c;
                    ResourceColorRect.Fill = r.Brush;
                }
            }
        }

        private void ResourceNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ResourcesList.SelectedItem is VResource r)
                r.Name = ResourceNameBox.Text;
        }
    }
}
