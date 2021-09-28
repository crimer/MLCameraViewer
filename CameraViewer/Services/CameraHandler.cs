using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using CameraViewer.MlNet;
using CameraViewer.MlNet.DataModels;
using CameraViewer.MlNet.DataModels.TinyYolo;
using CameraViewer.MlNet.YoloParser;
using CameraViewer.Models;
using CameraViewer.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;
using Pen = System.Drawing.Pen;

namespace CameraViewer.Services
{
    public class CameraHandler
    {
        private readonly Predictor _predictor;
        private readonly ILogger<CameraHandler> _logger;
        private Camera _camera;
        private IVideoSource _videoSource;
        private OnnxOutputParser outputParser;
        private readonly PredictionEngine<ImageInputData, TinyYoloPrediction> _predictionEngine;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="predictor">Предсказатель</param>
        /// <param name="logger">Логгер</param>
        public CameraHandler(Predictor predictor, ILogger<CameraHandler> logger)
        {
            _predictor = predictor;
            _logger = logger;
        }

        /// <summary>
        /// Подключиться к камере
        /// </summary>
        /// <param name="camera">Камера</param>
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
        
        /// <summary>
        /// Отключиться от камеры
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if(_videoSource == null)
                    return;
                
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= VideoSourceOnNewFrame;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Во отключения от камеры произошла ошибка: {ex}");
            }
        }
        
        /// <summary>
        /// Получение фрейма
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="eventArgs">Событие получения фрейма</param>
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
                _logger.LogError($"В событии получения фрейма произошла ошибка: {ex}");
            }
        }

        /// <summary>
        /// Парсинг фрейма с камеры
        /// </summary>
        /// <param name="bitmap">Фрейм</param>
        private void ParseWebCamFrame(Bitmap bitmap)
        {
            try
            {
                var filteredBoxes = DetectObjectsUsingModel(bitmap);
                if(filteredBoxes.IsNullOrEmpty())
                    filteredBoxes = new List<BoundingBox>();
                
                // Application.Current.Dispatcher.Invoke(() =>
                // {
                DrawOverlays(filteredBoxes, _camera.BitmapImage.Height, _camera.BitmapImage.Width);
                // });
            }
            catch (Exception ex)
            {
                _logger.LogError($"В парсинге фрейма камеры произошла ошибка: {ex}");
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
        /// Отрисовка границ найденых объектов
        /// </summary>
        /// <param name="filteredBoxes">Найденые границы объектов</param>
        /// <param name="originalHeight">Оригинальная высота</param>
        /// <param name="originalWidth">Оригинальная ширина</param>
        private void DrawOverlays(List<BoundingBox> filteredBoxes, double originalHeight, double originalWidth)
        {
            try
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
                
                    using (Graphics thumbnailGraphic = Graphics.FromImage(_camera.BitmapImage.ToBitmap()))
                    {
                        thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                        thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                        thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        // Define Text Options
                        Font drawFont = new Font("Arial", 12, FontStyle.Bold);
                        SizeF size = thumbnailGraphic.MeasureString(box.Description, drawFont);
                        Brush fontBrush = new SolidBrush(Color.Black);
                        Point atPoint = new Point((int)x, (int)y - (int)size.Height - 1);
                        
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
            catch (Exception ex)
            {
                _logger.LogError($"Во время отрисовки найденых объектов произошла ошибка: {ex}");
            }
        }
    }
}