using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using DofusBuddy.Managers;
using DofusBuddy.Settings;
using DofusBuddy.ViewModels;
using DofusBuddy.Views;
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
            try
            {
                EnsureConfigurationExists();
                ServiceProvider = ConfigureServices();
                InitializeComponent();
            }
            catch (Exception ex)
            {
                // Log the exception to a file
                File.WriteAllText(Path.Combine(_dofusBuddyAppDataFolderPath, "DofusBuddy_AppConstructor_Error.txt"), ex.ToString());
                throw;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // Log the exception to a file
                File.WriteAllText(Path.Combine(_dofusBuddyAppDataFolderPath, "DofusBuddy_Startup_Error.txt"), ex.ToString());
                throw;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveAppSettings();
        }

        private void SaveAppSettings()
        {
            ApplicationSettings appSettings = ServiceProvider.GetService<IOptions<ApplicationSettings>>()!.Value;

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

            services.AddTransient<MainWindow>();
            services.AddTransient<MainPage>();
            services.AddTransient<AddCharacterView>();
            services.AddTransient<SettingsView>();

            services.AddSingleton<HookManager>();
            services.AddSingleton<WindowManager>();
            services.AddSingleton<CharacterManager>();
            services.AddSingleton<PacketManager>();
            services.AddSingleton<GameManager>();
            services.AddSingleton<KeyboardManager>();

            services.AddTransient<MainPageViewModel>();
            services.AddTransient<AddCharacterViewModel>();
            services.AddTransient<SettingsViewModel>();

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
