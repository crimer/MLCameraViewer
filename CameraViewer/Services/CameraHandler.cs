using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CameraViewer.MlNet;
using CameraViewer.MlNet.DataModels;
using CameraViewer.MlNet.DataModels.TinyYolo;
using CameraViewer.MlNet.YoloParser;
using CameraViewer.Models;
using CameraViewer.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using FontStyle = System.Drawing.FontStyle;
using Pen = System.Drawing.Pen;
using Point = System.Drawing.Point;

namespace CameraViewer.Services
{
    public class CameraHandler
    {
        private readonly Predictor _predictor;
        private readonly ILogger<CameraHandler> _logger;
        private Camera _camera;
        private OnnxOutputParser outputParser;
        private CancellationTokenSource _cameraCancellationToken;
        private readonly PredictionEngine<ImageInputData, TinyYoloPrediction> _predictionEngine;

        private Mat _mat = new Mat();
        private VideoCapture _capture;

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
        public void Connect(Camera camera)
        {
            _cameraCancellationToken = new CancellationTokenSource();
            Task.Run(() => CaptureCamera(_cameraCancellationToken.Token), _cameraCancellationToken.Token);
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

        /// <summary>
        /// Получение фрейма
        /// </summary>
        /// <param name="token">Токен отмены</param>
        private async Task CaptureCamera(CancellationToken token)
        {
            try
            {
                if (_capture == null)
                    _capture = new VideoCapture();

                _capture.Open(0);
                if (_capture.IsOpened())
                {
                    while (!token.IsCancellationRequested)
                    {
                        _capture.Read(_mat);
                        if(_mat == null)
                            break;
                        
                        using (MemoryStream memoryStream = _capture.RetrieveMat().Flip(FlipMode.Y).ToMemoryStream())
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                _camera.BitmapImage = BitmapConverter.ToBitmap(_mat).ToBitmapImage();
                            });

                            var bitmapImage = new Bitmap(memoryStream);

                            await ParseWebCamFrame(bitmapImage, token);
                        }
                    }
                    _capture.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"В событии получения фрейма произошла ошибка: {ex}");
            }
        }

        private async Task ParseWebCamFrame(Bitmap bitmap, CancellationToken token)
        {
            var filteredBoxes = DetectObjectsUsingModel(bitmap);

            if (!token.IsCancellationRequested)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // DrawOverlays(filteredBoxes, WebCamImage.ActualHeight, WebCamImage.ActualWidth);
                    DrawOverlays(filteredBoxes, _camera.BitmapImage.Height, _camera.BitmapImage.Width);
                });
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