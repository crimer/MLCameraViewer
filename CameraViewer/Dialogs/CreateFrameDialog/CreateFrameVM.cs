using GalaSoft.MvvmLight;

namespace CameraViewer.Dialogs.CreateFrameDialog
{
    public class CreateFrameVM : ObservableObject
    {
        private string _ip = "127.0.0.1";
        public string Ip { get => _ip; set => Set(ref _ip, value); }
        
        
        private string _port = "1";
        public string Port { get => _port; set => Set(ref _port, value); }
        

        private string _name = "test";
        public string Name { get => _name; set => Set(ref _name, value); }
    }
}