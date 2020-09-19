using System;
using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.DataPrediction
{
    public class DataPredictionSpec
    {
        public DataPredictionSpec(string name, IReadOnlyList<DataPoint> trainData, IReadOnlyList<DataPoint> testData)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TrainData = trainData ?? throw new ArgumentNullException(nameof(trainData));
            TestData = testData ?? throw new ArgumentNullException(nameof(testData));
        }

        public string Name { get; }
        public IEnumerable<DataPoint> Data => TrainData.Concat(TestData).OrderBy(dataPoint => dataPoint.IndependentValue);
        public IReadOnlyList<DataPoint> TrainData { get; }
        public IReadOnlyList<DataPoint> TestData { get; }
    }
}