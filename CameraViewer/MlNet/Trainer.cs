using System.Collections.Generic;
using CameraViewer.MlNet.DataModels;
using Microsoft.ML;
using Microsoft.ML.Transforms.Image;

namespace CameraViewer.MlNet
{
    public class Trainer
    {
        public MLContext MlContext;
        private readonly ITransformer _mlModel;

        public Trainer(IOnnxModel onnxModel)
        {
            MlContext = new MLContext();
            // _mlModel = _mlContext.Model.Load(@"C:\Users\shevn\Desktop\CameraViewer\CameraViewer\bin\Debug\net472\MlNet\OnnxModels\model.zip", out var modelSchema);
            // _mlModel = SetupMlNetModel(onnxModel);
        }

        public ITransformer SetupModel(IOnnxModel onnxModel)
        {
            var dataView = MlContext.Data.LoadFromEnumerable(new List<ImageInputData>());
            
            var pipeline = MlContext.Transforms
                .ResizeImages(
                    resizing: ImageResizingEstimator.ResizingKind.Fill, 
                    outputColumnName: onnxModel.ModelInput, 
                    imageWidth: ImageSettings.ImageWidth, 
                    imageHeight: ImageSettings.ImageHeight, 
                    inputColumnName: nameof(ImageInputData.Image));
            
            pipeline
                .Append(MlContext.Transforms
                    .ExtractPixels(onnxModel.ModelInput));
            
            pipeline
                .Append(MlContext.Transforms
                    .ApplyOnnxModel(
                        modelFile: onnxModel.ModelPath,
                        outputColumnName: onnxModel.ModelOutput, 
                        inputColumnName: onnxModel.ModelInput));

            var mlNetModel = pipeline.Fit(dataView);
            // MlContext.Model.Save(_mlModel, dataView.Schema, "model2.zip");
            return mlNetModel;
        }

        public void SaveModel(string mlnetModelFilePath)
        {
            // Save/persist the model to a .ZIP file to be loaded by the PredictionEnginePool
            MlContext.Model.Save(_mlModel, null, mlnetModelFilePath);
        }
        
        public void LoadModel(string mlnetModelFilePath)
        {
            // Save/persist the model to a .ZIP file to be loaded by the PredictionEnginePool
            MlContext.Model.Save(_mlModel, null, mlnetModelFilePath);
        }
    }
}