using Microsoft.ML.Data;

namespace CameraViewer.MlNet.DataModels.TinyYolo
{
    public class TinyYoloPrediction : IOnnxObjectPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels { get; set; }
    }
}   