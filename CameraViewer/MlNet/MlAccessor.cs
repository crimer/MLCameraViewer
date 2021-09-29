using CameraViewer.MlNet.DataModels.TinyYolo;
using CameraViewer.MlNet.YoloParser;
using Microsoft.ML;

namespace CameraViewer.MlNet
{
    public class MlAccessor
    {
        private readonly Trainer _trainer;
        private readonly Predictor _predictor;
        private readonly TinyYoloModel _tinyYoloModel;
        private readonly OnnxOutputParser _onnxOutputParser;

        public Trainer Trainer => _trainer;
        public Predictor Predictor => _predictor;
        public TinyYoloModel TinyYoloModel => _tinyYoloModel;
        public OnnxOutputParser OnnxOutputParser => _onnxOutputParser;

        public MlAccessor(Trainer trainer, Predictor predictor)
        {
            _trainer = trainer;
            _predictor = predictor;
            _tinyYoloModel = new TinyYoloModel("TinyYolo2_model.onnx");
            _onnxOutputParser = new OnnxOutputParser(_tinyYoloModel);
        }

        public ITransformer SetupModel()
        {
            return _trainer.SetupModel(_tinyYoloModel);
        }
    }
}