using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CameraViewer.Pages.Home
{
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<HomeVM>();
        }
    }
}