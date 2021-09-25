using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AForge.Video;
using AForge.Video.DirectShow;
using CameraViewer.Models;

namespace CameraViewer.Services
{
    public class VideoService
    {
        private FilterInfoCollection _videoDevicesList;
        public Dictionary<Guid, Camera> Cameras;

        private IVideoSource _videoSource;
        
        
        public VideoService()
        {
        }

        


    }
}