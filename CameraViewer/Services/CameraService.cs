using System.Collections.Generic;
using System.Linq;
using AForge.Video.DirectShow;
using CameraViewer.Models;
using Hompus.VideoInputDevices;

namespace CameraViewer.Services
{
    /// <summary>
    /// Сервис по работе с камерами
    /// </summary>
    public class CameraService
    {
        /// <summary>
        /// Коллекция устройств
        /// </summary>
        private FilterInfoCollection _videoDevicesList;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public CameraService()
        {
            _videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }
        
        /// <summary>
        /// Получение всех камер
        /// </summary>
        /// <returns>Коллекция камер</returns>
        public List<WebCameraInfo> GetCameras()
        {
            var webCameraInfos = new List<WebCameraInfo>();

            // foreach (FilterInfo camera in _videoDevicesList)
            //     webCameraInfos.Add(new WebCameraInfo(camera.Name, camera.MonikerString));
            
            using (var sde = new SystemDeviceEnumerator())
            {
                var devices = sde.ListVideoInputDevice();
                var index = devices.First(d => d.Value == "Cam Link 4K").Key;
                // capture = new VideoCapture(index, VideoCapture.API.DShow);
            }
            
            return webCameraInfos;
        }
    }
}