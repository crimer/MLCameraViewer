namespace CameraViewer.Models
{
    public class WebCameraInfo
    {
        public string CameraName { get; }
        public int CameraIndex { get; }

        public WebCameraInfo(string cameraName, int cameraIndex)
        {
            CameraName = cameraName;
            CameraIndex = cameraIndex;
        }
    }
}