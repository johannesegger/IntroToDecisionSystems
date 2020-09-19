using System.Collections.Generic;

namespace DecisionSystems.DataPrediction.Predictor
{
    public interface IDataPredictor
    {
        IDataPredictionModel Train(IReadOnlyList<DataPoint> data);
    }
}