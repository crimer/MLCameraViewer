using CameraViewer.MlNet.DataModels.TinyYolo;
using CameraViewer.MlNet.YoloParser;
using Microsoft.ML;

namespace CameraViewer.MlNet
{
    /// <summary>
    /// Провайдер ML.Net
    /// </summary>
    public class MlAccessor
    {
        private readonly Trainer _trainer;
        private readonly Predictor _predictor;
        private readonly TinyYoloModel _tinyYoloModel;
        private readonly OnnxOutputParser _onnxOutputParser;

        /// <summary>
        /// Тренер
        /// </summary>
        public Trainer Trainer => _trainer;
        
        /// <summary>
        /// Предсказатель
        /// </summary>
        public Predictor Predictor => _predictor;
        
        /// <summary>
        /// Парсер ONNX модели
        /// </summary>
        public OnnxOutputParser OnnxOutputParser => _onnxOutputParser;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="trainer">Тренер</param>
        /// <param name="predictor">Предсказатель</param>
        public MlAccessor(Trainer trainer, Predictor predictor)
        {
            _trainer = trainer;
            _predictor = predictor;
            _tinyYoloModel = new TinyYoloModel("TinyYolo2_model.onnx");
            _onnxOutputParser = new OnnxOutputParser(_tinyYoloModel);
        }

        /// <summary>
        /// Запуск конфигурыции тодели
        /// </summary>
        /// <returns>ML трансформер</returns>
        public ITransformer SetupModel()
        {
            return _trainer.SetupModel(_tinyYoloModel);
        }
    }
}