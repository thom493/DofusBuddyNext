using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using DofusBuddy.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DofusBuddy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string _appSettingsFileName = "appsettings.json";
        private static readonly string _dofusBuddyAppDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dofus.Buddy");
        private static readonly string _dofusBuddyAppsettingsPath = Path.Combine(_dofusBuddyAppDataFolderPath, _appSettingsFileName);

        public IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            EnsureConfigurationExists();

            IConfigurationRoot configuration = GetConfiguration();

            ServiceProvider serviceProvider = ConfigureServices(configuration);

            MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();

            mainWindow.Show();
        }

        private static void EnsureConfigurationExists()
        {
            Directory.CreateDirectory(_dofusBuddyAppDataFolderPath);

            if (!File.Exists(_dofusBuddyAppsettingsPath))
            {
                CreateDefaultAppSettingsFile();
            }
        }

        private static void CreateDefaultAppSettingsFile()
        {
            using FileStream fileStream = File.Create(_dofusBuddyAppsettingsPath);

            var defaultSettings = new
            {
                ApplicationSettings = new ApplicationSettings()
            };

            string json = JsonSerializer.Serialize(defaultSettings);

            fileStream.Write(Encoding.UTF8.GetBytes(json));
        }

        private ServiceProvider ConfigureServices(IConfigurationRoot configuration)
        {
            var services = new ServiceCollection();

            services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(MainPage));

            return services.BuildServiceProvider();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(_dofusBuddyAppDataFolderPath)
                .AddJsonFile(_appSettingsFileName, optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }
}
