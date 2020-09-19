using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.DataPrediction.Predictor
{
    public interface IDataPredictionModel
    {
        double Test(double independentValue);

        List<DataPoint> Test(IEnumerable<double> independentValues)
        {
            return independentValues
                .Select(v => new DataPoint(v, Test(v)))
                .ToList();
        }
    }
}