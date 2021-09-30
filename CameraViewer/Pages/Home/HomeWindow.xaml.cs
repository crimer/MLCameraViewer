using System.Windows;

namespace CameraViewer.Pages.Home
{
    public partial class HomeWindow : Window
    {
        public HomeWindow(HomeVM homeVm)
        {
            InitializeComponent();
            DataContext = homeVm;
        }
    }
}