﻿using System;
using System.IO;
using System.Windows;
using CameraViewer.Configuration;
using CameraViewer.Pages.Home;
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

            var mainWindow = new HomeWindow();
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
            services.AddSingleton<CameraService>();
        }
    }
}