using System;
using System.Collections.Generic;
using System.Drawing;
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
using Path = System.IO.Path;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace CameraViewer.Services
{
    public class CameraHandler
    {
        private Camera _camera;
        private IVideoSource _videoSource;
        private OnnxOutputParser outputParser;
        private PredictionEngine<ImageInputData, TinyYoloPrediction> tinyYoloPredictionEngine;
        private static readonly string modelsDirectory = Path.Combine(Environment.CurrentDirectory, @"MlNet\OnnxModels");

        private void LoadModel()
        {
            var tinyYoloModel = new TinyYoloModel(Path.Combine(modelsDirectory, "TinyYolo2_model.onnx"));
            var modelConfigurator = new OnnxModelConfigurator(tinyYoloModel);

            outputParser = new OnnxOutputParser(tinyYoloModel);
            tinyYoloPredictionEngine = modelConfigurator.GetMlNetPredictionEngine<TinyYoloPrediction>();
        }
        
        public void Connect(Camera camera)
        {
            LoadModel();
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
                    
                    ParseWebCamFrame(bitmap);
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

            Application.Current.Dispatcher.Invoke(() =>
            {
                DrawOverlays(filteredBoxes, _camera.BitmapImage.Height, _camera.BitmapImage.Width);
            });
        }

        public List<BoundingBox> DetectObjectsUsingModel(ImageInputData imageInputData)
        {
            var labels = tinyYoloPredictionEngine?.Predict(imageInputData).PredictedLabels;
            var boundingBoxes = outputParser.ParseOutputs(labels);
            var filteredBoxes = outputParser.FilterBoundingBoxes(boundingBoxes, 5, 0.5f);
            return filteredBoxes;
        }
        
        private void DrawOverlays(List<BoundingBox> filteredBoxes, double originalHeight, double originalWidth)
        {
            WebCamCanvas.Children.Clear();
            
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
            
                var boxColor = box.BoxColor.ToMediaColor();
                
                var objBox = new Rectangle
                {
                    Width = width,
                    Height = height,
                    Fill = new SolidColorBrush(Colors.Transparent),
                    Stroke = new SolidColorBrush(boxColor),
                    StrokeThickness = 2.0,
                    Margin = new Thickness(x, y, 0, 0)
                };

                var objDescription = new TextBlock
                {
                    Margin = new Thickness(x + 4, y + 4, 0, 0),
                    Text = box.Description,
                    FontWeight = FontWeights.Bold,
                    Width = 126,
                    Height = 21,
                    TextAlignment = TextAlignment.Center
                };

                var objDescriptionBackground = new Rectangle
                {
                    Width = 134,
                    Height = 29,
                    Fill = new SolidColorBrush(boxColor),
                    Margin = new Thickness(x, y, 0, 0)
                };

                WebCamCanvas.Children.Add(objDescriptionBackground);
                WebCamCanvas.Children.Add(objDescription);
                WebCamCanvas.Children.Add(objBox);
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