using System.Drawing;
using Microsoft.ML.Transforms.Image;

namespace CameraViewer.MlNet.DataModels
{
    /// <summary>
    /// Размеры фрейма
    /// </summary>
    public struct ImageSettings
    {
        /// <summary>
        /// Высота изображения
        /// </summary>
        public const int ImageHeight = 416;
        
        /// <summary>
        /// Длинна изображения
        /// </summary>
        public const int ImageWidth = 416;
    }

    /// <summary>
    /// Входные данные изображения
    /// </summary>
    public class ImageInputData
    {
        /// <summary>
        /// Bitmap изображения
        /// </summary>
        [ImageType(ImageSettings.ImageHeight, ImageSettings.ImageWidth)]
        public Bitmap Image { get; set; }
    }
}