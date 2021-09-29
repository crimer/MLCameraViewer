using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CameraViewer.MlNet;
using CameraViewer.MlNet.DataModels;
using CameraViewer.MlNet.DataModels.TinyYolo;
using CameraViewer.MlNet.YoloParser;
using CameraViewer.Models;
using CameraViewer.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using OpenCvSharp;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace CameraViewer.Components
{
    public partial class CameraFrame : UserControl
    {
        public static readonly DependencyProperty CameraProperty = 
            DependencyProperty.Register(
            "Camera",
            typeof(Camera), 
            typeof(CameraFrame),
            new PropertyMetadata(default(Camera)));

        public Camera Camera
        {
            get { return (Camera) GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }
        
        private readonly Predictor _predictor;
        private readonly ILogger<CameraFrame> _logger;
        
        private OnnxOutputParser outputParser;
        private CancellationTokenSource _cameraCancellationToken;
        private VideoCapture _capture;
        // private readonly PredictionEngine<ImageInputData, TinyYoloPrediction> _predictionEngine;


        public CameraFrame()
        {
            InitializeComponent();
            _predictor = App.ServiceProvider.GetRequiredService<Predictor>();
            _logger = App.ServiceProvider.GetRequiredService<ILogger<CameraFrame>>();
            // Создавать 
            outputParser = new OnnxOutputParser();
        }

        /// <summary>
        /// Подключиться к камере
        /// </summary>
        /// <param name="camera">Камера</param>
        public async void Connect(Camera camera)
        {
            _cameraCancellationToken = new CancellationTokenSource();
            Task.Run(() => CaptureCamera(camera, _cameraCancellationToken.Token), _cameraCancellationToken.Token);
        }
        
        
        private async Task CaptureCamera(Camera camera, CancellationToken token)
        {
            if (_capture == null)
                _capture = new VideoCapture(camera.CameraIndex);

            _capture.Open(camera.CameraIndex);

            if (_capture.IsOpened())
            {
                while (!token.IsCancellationRequested)
                {
                    using (MemoryStream memoryStream = _capture.RetrieveMat().Flip(FlipMode.Y).ToMemoryStream())
                    {

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            var imageSource = new BitmapImage();

                            imageSource.BeginInit();
                            imageSource.CacheOption = BitmapCacheOption.OnLoad;
                            imageSource.StreamSource = memoryStream;
                            imageSource.EndInit();

                            WebCamImage.Source = imageSource;
                        });

                        var bitmapImage = new Bitmap(memoryStream);

                        await ParseWebCamFrame(bitmapImage, token);
                    }
                }

                _capture.Release();
            }
        }

        private async Task ParseWebCamFrame(Bitmap bitmapImage, CancellationToken token)
        {
            var filteredBoxes = DetectObjectsUsingModel(bitmapImage);

            if (!token.IsCancellationRequested)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DrawOverlays(filteredBoxes, WebCamImage.ActualHeight, WebCamImage.ActualWidth);
                });
            }
        }

        private void DrawOverlays(List<BoundingBox> filteredBoxes, double originalHeight, double originalWidth)
        {
            WebCamCanvas.Children.Clear();

            foreach (var box in filteredBoxes)
            {
                // process output boxes
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

        /// <summary>
        /// Обнаружение объектов на фрейме
        /// </summary>
        /// <param name="bitmap">Фрейм</param>
        /// <returns>Коллекция найденных объектов</returns>
        private List<BoundingBox> DetectObjectsUsingModel(Bitmap bitmap)
        {
            try
            {
                var label = _predictor.Predict(bitmap);
                var labels = label?.PredictedLabels;
                if (labels.IsNullOrEmpty())
                    return new List<BoundingBox>();
            
                var boundingBoxes = outputParser.ParseOutputs(labels);
                var filteredBoxes = outputParser.FilterBoundingBoxes(boundingBoxes, 5, 0.5f);
                return filteredBoxes;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Во время нахождения объектов на камере произошла ошибка: {ex}");
                return new List<BoundingBox>();
            }
        }
        
        /// <summary>
        /// Отключиться от камеры
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _cameraCancellationToken?.Cancel();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Во отключения от камеры произошла ошибка: {ex}");
            }
        }

        private void OnConnectClick(object sender, RoutedEventArgs e)
        {
            Connect(Camera);
        }

        private void OnDisconnectClick(object sender, RoutedEventArgs e)
        {
            Disconnect();
            // CamerasCollection.Remove(camera);
        }
    }
}