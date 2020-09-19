using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.DataPrediction
{
    public static class Utils
    {
        public static double SquaredDiff(double a, double b)
        {
            var diff = a - b;
            return diff * diff;
        }

        public static double MeanSquaredError(IEnumerable<DataPoint> dataPoints, IEnumerable<double> prediction)
        {
            return dataPoints
                .Select(dataPoint => dataPoint.DependentValue)
                .Zip(prediction, SquaredDiff)
                .Average();
        }
    }
}