using Microsoft.ML.Data;

namespace MlHumanDetection.ML.DataModels
{
    public class TinyYoloPrediction : IOnnxObjectPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels { get; set; }
    }
}   