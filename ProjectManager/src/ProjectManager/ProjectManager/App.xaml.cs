using Microsoft.Extensions.DependencyInjection;
using ProjectManager.Models;
using ProjectManager.ViewModels;
using ProjectManager.Services;
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
            _ = services.AddSingleton<ProjectStorage>();
            _ = services.AddSingleton<ProjectResourceStorage>();
            _ = services.AddSingleton(sp => new SqliteStorageService(
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ProjectManager", "projectmanager.db"),
                sp.GetRequiredService<Settings>(),
                sp.GetRequiredService<ProjectStorage>(),
                sp.GetRequiredService<ProjectResourceStorage>()));

            _ = services.AddTransient<MainViewModel>();
            _ = services.AddTransient<TimeLineViewViewModel>();
            _ = services.AddTransient<ProjectViewViewModel>();
            _ = services.AddTransient<MainWindow>();

            _provider = services.BuildServiceProvider();

            var mainWindow = _provider.GetRequiredService<MainWindow>();

            mainWindow.Show();
        }
    }
}
