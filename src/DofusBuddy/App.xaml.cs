using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace DofusBuddy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(MainWindow));
        }
    }
}
