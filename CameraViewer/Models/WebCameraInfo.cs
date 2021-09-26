namespace CameraViewer.Models
{
    public class WebCameraInfo
    {
        public string CameraName { get; }
        public string CameraMonikerString { get; }

        public WebCameraInfo(string cameraName, string cameraMonikerString)
        {
            CameraName = cameraName;
            CameraMonikerString = cameraMonikerString;
        }
    }
}