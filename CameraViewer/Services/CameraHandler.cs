using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using CameraViewer.MlNet;
using CameraViewer.MlNet.DataModels;
using CameraViewer.MlNet.YoloParser;
using CameraViewer.Models;
using CameraViewer.Utils;
using Microsoft.ML;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;
using Path = System.IO.Path;
using Pen = System.Drawing.Pen;

namespace CameraViewer.Services
{
    public class CameraHandler
    {
        private Camera _camera;
        private IVideoSource _videoSource;
        private OnnxOutputParser outputParser;
        private PredictionEngine<ImageInputData, TinyYoloPrediction> tinyYoloPredictionEngine;
        private static readonly string modelsDirectory = Path.Combine(Environment.CurrentDirectory, @"MlNet\OnnxModels");

        public CameraHandler()
        {
            LoadModel();
        }
        
        private void LoadModel()
        {
            var tinyYoloModel = new TinyYoloModel(Path.Combine(modelsDirectory, "TinyYolo2_model.onnx"));
            var modelConfigurator = new OnnxModelConfigurator(tinyYoloModel);

            outputParser = new OnnxOutputParser(tinyYoloModel);
            // var transformer = modelConfigurator.MlContext.Model.Load(Path.Combine(modelsDirectory, "model2.zip"), out var scheeme);
            tinyYoloPredictionEngine = modelConfigurator.GetMlNetPredictionEngine<TinyYoloPrediction>();
            // tinyYoloPredictionEngine = modelConfigurator.MlContext.Model.CreatePredictionEngine<ImageInputData, TinyYoloPrediction>(transformer, scheeme);
        }
        
        public async Task Connect(Camera camera)
        {
            await Task.Run(() =>
            {
                _camera = camera;
                _videoSource = new VideoCaptureDevice(camera.MonikerString);
                _videoSource.NewFrame += VideoSourceOnNewFrame;
                _videoSource.Start();
            });
        }

        private void VideoSourceOnNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (var bitmap = (Bitmap) eventArgs.Frame.Clone())
                {
                    var bi = bitmap.ToBitmapImage();
                    bi.Freeze();
                        
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        _camera.BitmapImage = bi;
                        ParseWebCamFrame(bitmap);
                    });
                
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При создании камеры произошла ошибка: {ex}");
            }
        }


        private void ParseWebCamFrame(Bitmap bitmap)
        {
            var frame = new ImageInputData { Image = bitmap };
            var filteredBoxes = DetectObjectsUsingModel(frame);
            if(filteredBoxes.IsNullOrEmpty())
                filteredBoxes = new List<BoundingBox>();
                
            // Application.Current.Dispatcher.Invoke(() =>
            // {
                DrawOverlays(filteredBoxes, _camera.BitmapImage.Height, _camera.BitmapImage.Width);
            // });
        }

        public List<BoundingBox> DetectObjectsUsingModel(ImageInputData imageInputData)
        {
            var label = tinyYoloPredictionEngine.Predict(imageInputData);
            var labels = label?.PredictedLabels;
            if (labels.IsNullOrEmpty())
                return new List<BoundingBox>();
            
            var boundingBoxes = outputParser.ParseOutputs(labels);
            var filteredBoxes = outputParser.FilterBoundingBoxes(boundingBoxes, 5, 0.5f);
            return filteredBoxes;
        }
        
        private void DrawOverlays(List<BoundingBox> filteredBoxes, double originalHeight, double originalWidth)
        {
            foreach (var box in filteredBoxes)
            {
                double x = Math.Max(box.Dimensions.X, 0);
                double y = Math.Max(box.Dimensions.Y, 0);
                double width = Math.Min(originalWidth - x, box.Dimensions.Width);
                double height = Math.Min(originalHeight - y, box.Dimensions.Height);
            
                // fit to current image size
                x = originalWidth * x / ImageSettings.ImageWidth;
                y = originalHeight * y / ImageSettings.ImageHeight;
                width = originalWidth * width / ImageSettings.ImageWidth;
                height = originalHeight * height / ImageSettings.ImageHeight;
            
                // var boxColor = box.BoxColor.ToMediaColor();
                
                using (Graphics thumbnailGraphic = Graphics.FromImage(_camera.BitmapImage.ToBitmap()))
                {
                    thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                    thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                    thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Define Text Options
                    Font drawFont = new Font("Arial", 12, FontStyle.Bold);
                    SizeF size = thumbnailGraphic.MeasureString(box.Description, drawFont);
                    Brush fontBrush = new SolidBrush(Color.Black);
                    System.Drawing.Point atPoint = new System.Drawing.Point((int)x, (int)y - (int)size.Height - 1);
                    
                    // Define BoundingBox options
                    Pen pen = new Pen(box.BoxColor, 3.2f);

                    // Draw text on image 
                    thumbnailGraphic.FillRectangle(
                        new SolidBrush(box.BoxColor), 
                        (int)x, 
                        (int)(y - size.Height - 1),
                        (int)size.Width, (int)size.Height);
                    
                    thumbnailGraphic.DrawString(box.Description, drawFont, fontBrush, atPoint);

                    // Draw bounding box on image
                    thumbnailGraphic.DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
                }
            }
        }
        
        public void Disconnect()
        {
            if(_videoSource == null)
                return;
            
            _videoSource.SignalToStop();
            _videoSource.NewFrame -= VideoSourceOnNewFrame;
        }
    }
}