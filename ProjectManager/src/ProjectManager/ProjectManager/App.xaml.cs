using Microsoft.Extensions.DependencyInjection;
using ProjectManager.Models;
using ProjectManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _provider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            _ = services.AddSingleton<Settings>();
            
            _ = services.AddTransient<TimeLineViewViewModel>();
            _ = services.AddTransient<MainWindow>();

            _provider = services.BuildServiceProvider();

            var mainWindow = _provider.GetRequiredService<MainWindow>();

            mainWindow.Show();
        }
    }
}
