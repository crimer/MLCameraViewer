using System.Drawing;
using CameraViewer.MlNet.DataModels;
using CameraViewer.MlNet.DataModels.TinyYolo;
using Microsoft.ML;

namespace CameraViewer.MlNet
{
    /// <summary>
    /// Предсказатель
    /// </summary>
    public class Predictor
    {
        private readonly Trainer _trainer;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="trainer">Тренер</param>
        public Predictor(Trainer trainer)
        {
            _trainer = trainer;
        }

        /// <summary>
        /// Получение движка предсказания
        /// </summary>
        /// <typeparam name="T">Тип выходного предсказания</typeparam>
        /// <returns>Движок предсказаний</returns>
        public PredictionEngine<ImageInputData, T> GetPredictionEngine<T>() where T : class, IOnnxPrediction, new()
        {
            return _trainer.MlContext.Model.CreatePredictionEngine<ImageInputData, T>(_trainer.MlModel);
        }
        
        /// <summary>
        /// Предсказать
        /// </summary>
        /// <param name="predictionEngine">Движок предсказания</param>
        /// <param name="frame">Bitmap фрейма</param>
        /// <typeparam name="T">Тип выходного предсказания</typeparam>
        /// <returns>Предсказание</returns>
        public IOnnxPrediction Predict<T>(PredictionEngine<ImageInputData, T> predictionEngine, Bitmap frame) where T : class, IOnnxPrediction, new()
        {
            var inputData = new ImageInputData() {Image = frame};
            return predictionEngine.Predict(inputData);
        }
    }
}