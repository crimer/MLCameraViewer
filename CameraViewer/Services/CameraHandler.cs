using System;
using System.Drawing;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using CameraViewer.Models;
using CameraViewer.Utils;

namespace CameraViewer.Services
{
    public class CameraHandler
    {
        private Camera _camera;
        private IVideoSource _videoSource;

        public void Connect(Camera camera)
        {
            _camera = camera;
            _videoSource = new VideoCaptureDevice(camera.MonikerString);
            
            
            _videoSource.NewFrame += VideoServiceOnOnAcceptFrame;
            _videoSource.Start();
        }
        
        private void VideoServiceOnOnAcceptFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (var bitmap = (Bitmap) eventArgs.Frame.Clone())
                {
                    var bi = bitmap.ToBitmapImage();
                    bi.Freeze();
                    Dispatcher.CurrentDispatcher.Invoke(() => _camera.BitmapImage = bi);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При создании камеры произошла ошибка: {ex}");
            }
        }
        
        public void Disconnect()
        {
            if(_videoSource == null)
                return;
            
            _videoSource.SignalToStop();
        }
    }
}