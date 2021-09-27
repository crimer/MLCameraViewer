using System.Drawing;
using Microsoft.ML.Transforms.Image;

namespace CameraViewer.MlNet.DataModels
{
    /// <summary>
    /// Размеры фрейма
    /// </summary>
    public struct ImageSettings
    {
        public const int ImageHeight = 416;
        public const int ImageWidth = 416;
    }

    public class ImageInputData
    {
        [ImageType(ImageSettings.ImageHeight, ImageSettings.ImageWidth)]
        public Bitmap Image { get; set; }
    }
}