using System.Drawing;

namespace CameraViewer.MlNet.YoloParser
{
    public class BoundingBoxDimensions
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
    }

    public class BoundingBox
    {
        public BoundingBoxDimensions Dimensions { get; set; }

        public string Label { get; set; }

        public float Confidence { get; set; }

        public RectangleF Rect
        {
            get { return new RectangleF(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height); }
        }

        public Color BoxColor { get; set; }

        public string Description => $"{Label} ({(Confidence * 100).ToString("0")}%)";

        private static readonly Color[] classColors = new Color[]
        {
            Color.Red, Color.Red, Color.Red, Color.Red,
            Color.Red, Color.Red, Color.Red, Color.Red,
            Color.Red, Color.Red, Color.Red, Color.Red,
            Color.Red, Color.Red, Color.Red, Color.Red,
            Color.Red, Color.Red, Color.Red, Color.Red,
            Color.Red
        };

        public static Color GetColor(int index) => index < classColors.Length ? classColors[index] : classColors[index % classColors.Length];
    }
}