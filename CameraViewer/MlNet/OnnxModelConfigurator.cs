using System.Collections.Generic;
using CameraViewer.MlNet.DataModels;
using Microsoft.ML;
using Microsoft.ML.Transforms.Image;

namespace CameraViewer.MlNet
{
    public class OnnxModelConfigurator
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _mlModel;

        public OnnxModelConfigurator(IOnnxModel onnxModel)
        {
            _mlContext = new MLContext();
            _mlModel = SetupMlNetModel(onnxModel);
        }

        private ITransformer SetupMlNetModel(IOnnxModel onnxModel)
        {
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
                        modelFile: onnxModel.ModelPath,
                        outputColumnName: onnxModel.ModelOutput, 
                        inputColumnName: onnxModel.ModelInput));

            var mlNetModel = pipeline.Fit(dataView);

            return mlNetModel;
        }

        public PredictionEngine<ImageInputData, T> GetMlNetPredictionEngine<T>() where T : class, IOnnxObjectPrediction, new()
        {
            return _mlContext.Model.CreatePredictionEngine<ImageInputData, T>(_mlModel);
        }

        public void SaveMLNetModel(string mlnetModelFilePath)
        {
            // Save/persist the model to a .ZIP file to be loaded by the PredictionEnginePool
            _mlContext.Model.Save(_mlModel, null, mlnetModelFilePath);
        }
    }
}