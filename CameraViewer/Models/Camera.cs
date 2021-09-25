using System;
using System.Net;
using System.Windows.Media.Imaging;
using CameraViewer.Services;
using GalaSoft.MvvmLight;

namespace CameraViewer.Models
{
    public class Camera : ObservableObject
    {
        #region Props

        private Guid _id;
        public Guid Id { get => _id; set { Set(ref _id, value); } }
        
        private string _name;
        public string Name { get => _name; set { Set(ref _name, value); } }
        
        private IPAddress _ip;
        public IPAddress Ip { get => _ip; set { Set(ref _ip, value); } }
        
        private int _port;
        public int Port { get => _port; set { Set(ref _port, value); } }

        private BitmapImage _bitmapImage;
        public BitmapImage BitmapImage { get => _bitmapImage; set { Set(ref _bitmapImage, value); } }
        
        private bool _isRecording;
        public bool IsRecording { get => _isRecording; set { Set(ref _isRecording, value); } }
        public string MonikerString { get; set; }
        public CameraHandler Handler { get; private set; }

        #endregion

        /// <summary>
        /// Создание экземпляра камеры
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="ipaddress">Ip адрес</param>
        /// <param name="port">Порт</param>
        /// <returns>Камера</returns>
        public Camera Create(string name, string ipaddress, string port)
        {
            if (!IPAddress.TryParse(ipaddress, out var ip))
                throw new Exception($"Не корректный IP адрес камеры {name}");

            return new Camera()
            {
                Name = name,
                Ip = ip,
                Id = Guid.NewGuid(),
                Handler = new CameraHandler(),
                Port = int.Parse(port),
            };
        }
    }
}