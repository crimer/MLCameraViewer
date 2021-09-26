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
        
        private string _ip;
        public string Ip { get => _ip; set { Set(ref _ip, value); } }
        
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
        /// Конструктор
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="ipaddress">Ip адрес</param>
        /// <param name="port">Порт</param>
        /// <returns>Камера</returns>
        public Camera(string name, string ipaddress, string port)
        {
            Id = Guid.NewGuid();
            Name = name;
            Ip = ipaddress;
            Handler = new CameraHandler();
            Port = int.Parse(port);
        }
    }
}