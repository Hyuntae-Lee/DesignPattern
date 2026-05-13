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

namespace ProjectManager.Views
{
    /// <summary>
    /// Interaction logic for TimeLineView.xaml
    /// </summary>
    public partial class TimeLineView : UserControl
    {
        TimeLineViewViewModel _viewModel;

        const int kDateW = 30;

        public TimeLineView()
        {
            InitializeComponent();

            //
            _viewModel = (TimeLineViewViewModel)DataContext;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(new SolidColorBrush(Colors.Red), new Pen(), new Rect(0, 0, 30, 30));
        }
    }
}
