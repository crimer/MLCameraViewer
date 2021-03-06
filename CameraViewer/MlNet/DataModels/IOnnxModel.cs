namespace CameraViewer.MlNet.DataModels
{
    public interface IOnnxModel
    {
        string ModelName { get; }

        // To check Model input and output parameter names, you can
        // use tools like Netron: https://github.com/lutzroeder/netron
        string ModelInput { get; }
        string ModelOutput { get; }

        string[] Labels { get; }
        (float, float)[] Anchors { get; }
    }

    public interface IOnnxPrediction
    {
        float[] PredictedLabels { get; set; }
    }
}