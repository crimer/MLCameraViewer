using System.Drawing;
using CameraViewer.MlNet.DataModels;
using Microsoft.ML;

namespace CameraViewer.MlNet
{
    public class Predictor
    {
        private readonly Trainer _trainer;
        private MLContext _mLContext;
        private PredictionEngine<ImageInputData, ImagePrediction> _predictionEngine;

        public Predictor(Trainer trainer)
        {
            _trainer = trainer;
            _mLContext = trainer.MlContext;
            _mLContext = new MLContext();
            _predictionEngine = _mLContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(trainedModel);
        }

        public PredictionEngine<ImageInputData, T> GetPredictionEngine<T>() where T : class, IOnnxObjectPrediction, new()
        {
            return MlContext.Model.CreatePredictionEngine<ImageInputData, T>(_mlModel);
        }
        
        public ImagePrediction Predict(Bitmap image)
        {
            tinyYoloPredictionEngine.Predict(imageInputData);
            return _predictionEngine.Predict(new ImageData() { Image = image });
        }
    }
}