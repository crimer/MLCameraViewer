using System.Drawing;
using CameraViewer.MlNet.DataModels;
using CameraViewer.MlNet.DataModels.TinyYolo;
using Microsoft.ML;

namespace CameraViewer.MlNet
{
    public class Predictor
    {
        private readonly Trainer _trainer;

        public Predictor(Trainer trainer)
        {
            _trainer = trainer;
        }

        public PredictionEngine<ImageInputData, T> GetPredictionEngine<T>() where T : class, IOnnxObjectPrediction, new()
        {
            return _trainer.MlContext.Model.CreatePredictionEngine<ImageInputData, T>(_trainer.MlModel);
        }
        
        public IOnnxObjectPrediction Predict<T>(PredictionEngine<ImageInputData, T> predictionEngine, Bitmap image) where T : class, IOnnxObjectPrediction, new()
        {
            var inputData = new ImageInputData() {Image = image};
            return predictionEngine.Predict(inputData);
        }
    }
}