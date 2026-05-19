using ProjectManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(
            TimeLineViewViewModel timeLineViewViewModel,
            ProjectViewViewModel projectViewViewModel,
            MainViewModel mainViewModel)
        {
            InitializeComponent();

            DataContext = mainViewModel;
            timeLineView.DataContext = timeLineViewViewModel;
            projectView.DataContext = projectViewViewModel;
        }
    }
}
