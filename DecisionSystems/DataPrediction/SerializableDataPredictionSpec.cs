using System.Linq;

namespace DecisionSystems.DataPrediction
{
    public class SerializableDataPredictionSpec
    {
        public string Name { get; set; }
        public SerializableDataPoint[] TrainData { get; set; }
        public SerializableDataPoint[] TestData { get; set; }

        public static DataPredictionSpec ToDomain(SerializableDataPredictionSpec spec)
        {
            return new DataPredictionSpec(
                spec.Name,
                spec.TrainData.Select(SerializableDataPoint.ToDomain).ToList(),
                spec.TestData.Select(SerializableDataPoint.ToDomain).ToList()
            );
        }
    }
}