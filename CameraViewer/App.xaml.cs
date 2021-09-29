using System;
using System.IO;
using System.Windows;
using CameraViewer.Configuration;
using CameraViewer.Database;
using CameraViewer.MlNet;
using CameraViewer.MlNet.DataModels.TinyYolo;
using CameraViewer.Pages.Home;
using CameraViewer.Repositories.ClickHouse;
using CameraViewer.Repositories.Redis;
using CameraViewer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CameraViewer
{
    /// <summary>
    /// Точка входа приложения
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Провайдер сервисов
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }
 
        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        public IConfiguration Configuration { get; private set; }
 
        /// <summary>
        /// Запуск
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            Configuration = builder.Build();
            
            Configuration.GetSection("Config").Bind(new AppConfig());

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
 
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // ServiceProvider
            //     .GetRequiredService<Trainer>()
            //     .SetupModel(new TinyYoloModel("TinyYolo2_model.onnx"));

            var mainWindow = ServiceProvider.GetRequiredService<HomeWindow>();
            mainWindow.Show();
        }
 
        /// <summary>
        /// Конфигурация сервисов
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddConsole();
            });
            
            services.AddSingleton<HomeVM>();
            services.AddSingleton<HomeWindow>();
            
            services.AddSingleton<CameraService>();
            services.AddSingleton<CameraHandler>();
            
            services.AddSingleton<RedisDbProvider>();
            services.AddSingleton<ClickHouseDbProvider>();
            
            services.AddSingleton<RedisRepository>();
            services.AddSingleton<ClickHouseMetricRepository>();
            services.AddSingleton<ScriptsProvider>();
            
            services.AddSingleton<Predictor>();
            services.AddSingleton<Drawer>();
            services.AddSingleton<Trainer>();
        }
    }
}