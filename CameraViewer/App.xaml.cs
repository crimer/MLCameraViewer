using System;
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
    
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
 
        public IConfiguration Configuration { get; private set; }
 
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
 
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddConsole();
            });
            
            services.AddSingleton<HomeVM>();
            services.AddSingleton<VideoService>();
        }
    }
}