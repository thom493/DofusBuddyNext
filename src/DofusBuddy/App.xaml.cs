using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using DofusBuddy.Core;
using DofusBuddy.Core.Settings;
using DofusBuddy.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        private IServiceProvider ServiceProvider { get; set; }

        public App()
        {
            EnsureConfigurationExists();

            ServiceProvider = ConfigureServices();

            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveAppSettings();
        }

        private void SaveAppSettings()
        {
            ApplicationSettings appSettings = ServiceProvider.GetService<IOptions<ApplicationSettings>>().Value;

            var settings = new
            {
                ApplicationSettings = appSettings
            };

            string json = JsonSerializer.Serialize(settings);

            File.WriteAllText(_dofusBuddyAppsettingsPath, json);
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

        private static ServiceProvider ConfigureServices()
        {
            IConfigurationRoot configuration = GetConfiguration();

            var services = new ServiceCollection();

            services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainPage>();
            services.AddSingleton<MainPageViewModel>();

            return services.BuildServiceProvider();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(_dofusBuddyAppDataFolderPath)
                .AddJsonFile(_appSettingsFileName);

            return builder.Build();
        }
    }
}
