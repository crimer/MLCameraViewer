using System.Collections.Generic;
using System.Linq;
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
        /// Получение всех камер
        /// </summary>
        /// <returns>Коллекция камер</returns>
        public List<WebCameraInfo> GetCameras()
        {
            var webCameraInfos = new List<WebCameraInfo>();

            using (var sde = new SystemDeviceEnumerator())
            {
                var devices = sde.ListVideoInputDevice();
                webCameraInfos = devices
                    .Select(d => new WebCameraInfo(d.Value, d.Key))
                    .ToList();

            }
            
            return webCameraInfos;
        }
    }
}