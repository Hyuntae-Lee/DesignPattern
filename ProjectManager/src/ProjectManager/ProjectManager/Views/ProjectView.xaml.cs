using ProjectManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ProjectManager.Views
{
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
        }

        private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is ProjectViewViewModel vm)
                vm.SelectedItem = e.NewValue;
        }
    }
}
