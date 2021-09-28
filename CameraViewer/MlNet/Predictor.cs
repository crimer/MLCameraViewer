using System.Drawing;
using CameraViewer.MlNet.DataModels;
using CameraViewer.MlNet.DataModels.TinyYolo;
using Microsoft.ML;

namespace CameraViewer.MlNet
{
    public class Predictor
    {
        private readonly Trainer _trainer;
        private readonly PredictionEngine<ImageInputData, TinyYoloPrediction> _predictionEngine;

        public Predictor(Trainer trainer)
        {
            _trainer = trainer;
            _predictionEngine = GetPredictionEngine<TinyYoloPrediction>();
        }

        private PredictionEngine<ImageInputData, T> GetPredictionEngine<T>() where T : class, IOnnxObjectPrediction, new()
        {
            return _trainer.MlContext.Model.CreatePredictionEngine<ImageInputData, T>(_trainer.MlModel);
        }
        
        public IOnnxObjectPrediction Predict(Bitmap image)
        {
            return _predictionEngine.Predict(new ImageInputData() { Image = image });
        }
    }
}