using System.Collections.Generic;
using AForge.Video.DirectShow;
using CameraViewer.Models;

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

            foreach (FilterInfo camera in _videoDevicesList)
                webCameraInfos.Add(new WebCameraInfo(camera.Name, camera.MonikerString));
            
            return webCameraInfos;
        }
    }
}