using System;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;

namespace CameraViewer.Models
{
    public class Camera : ObservableObject
    {
        private Guid _id;
        public Guid Id { get => _id; set { Set(ref _id, value); } }
        
        private string _name;
        public string Name { get => _name; set { Set(ref _name, value); } }
        
        private BitmapImage _bitmapImage;
        public BitmapImage BitmapImage { get => _bitmapImage; set { Set(ref _bitmapImage, value); } }
        
        private bool _isRecording;
        public int CameraIndex { get; set; }
        // public CameraHandler Handler { get; private set; }

        // public static int Index;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <returns>Камера</returns>
        public Camera(WebCameraInfo selectedCamera)
        {
            Id = Guid.NewGuid();
            Name = selectedCamera.CameraName;
            CameraIndex = selectedCamera.CameraIndex;
        }

        public Camera()
        {
            
        }
    }
}