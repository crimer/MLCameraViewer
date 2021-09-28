using System;
using System.Collections.Generic;
using System.IO;
using CameraViewer.MlNet.DataModels;
using Microsoft.ML;
using Microsoft.ML.Transforms.Image;

namespace CameraViewer.MlNet
{
    public class Trainer
    {
        private readonly string _modelsDirectory = Path.Combine(Environment.CurrentDirectory, @"MlNet\OnnxModels");
        private MLContext _mlContext;
        private ITransformer _mlModel;

        public MLContext MlContext => _mlContext;
        public ITransformer MlModel => _mlModel;
        
        public Trainer()
        {
            _mlContext = new MLContext();
        }

        public ITransformer SetupModel(IOnnxModel onnxModel)
        {
            var onnxModelPath = Path.Combine(_modelsDirectory, "TinyYolo2_model.onnx");
            
            var dataView = _mlContext.Data.LoadFromEnumerable(new List<ImageInputData>());
            
            var pipeline = _mlContext.Transforms
                .ResizeImages(
                    resizing: ImageResizingEstimator.ResizingKind.Fill, 
                    outputColumnName: onnxModel.ModelInput, 
                    imageWidth: ImageSettings.ImageWidth, 
                    imageHeight: ImageSettings.ImageHeight, 
                    inputColumnName: nameof(ImageInputData.Image));
            
            pipeline
                .Append(_mlContext.Transforms
                    .ExtractPixels(onnxModel.ModelInput));
            
            pipeline
                .Append(_mlContext.Transforms
                    .ApplyOnnxModel(
                        modelFile: onnxModelPath,
                        outputColumnName: onnxModel.ModelOutput, 
                        inputColumnName: onnxModel.ModelInput));
            
            _mlModel = pipeline.Fit(dataView);
            return _mlModel;
        }

        /// <summary>
        /// Сохренение модели
        /// </summary>
        /// <param name="modelName">Название мидели</param>
        public void SaveModel(string modelName = "trainedModel.zip")
        {
            var modelPath = Path.Combine(_modelsDirectory, modelName);
            
            if(File.Exists(modelPath))
                File.Delete(modelPath);
            
            _mlContext.Model.Save(_mlModel, null, modelPath);
        }
        
        /// <summary>
        /// Загрузка модели
        /// </summary>
        /// <param name="modelName">Название мидели</param>
        /// <exception cref="Exception">Ошабка отсутствия модели</exception>
        public void LoadModel(string modelName)
        {
            var modelPath = Path.Combine(_modelsDirectory, modelName);
            if (!File.Exists(modelPath))
                throw new Exception($"Не удалось найти.zip файл модели по пути: {modelPath}");
            
            _mlContext.Model.Save(_mlModel, null, modelPath);
        }
    }
}