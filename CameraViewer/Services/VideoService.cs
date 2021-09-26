using System;
using System.Collections.Generic;
using AForge.Video.DirectShow;
using CameraViewer.Models;

namespace CameraViewer.Services
{
    public class VideoService
    {
        private FilterInfoCollection _videoDevicesList;
        
        public VideoService()
        {
            _videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public List<WebCameraInfo> GetCameras()
        { 
            var webCameraInfos = new List<WebCameraInfo>();

            foreach (FilterInfo camera in _videoDevicesList)
                webCameraInfos.Add(new WebCameraInfo(camera.Name, camera.MonikerString));
            
            return webCameraInfos;
        }
    }
}