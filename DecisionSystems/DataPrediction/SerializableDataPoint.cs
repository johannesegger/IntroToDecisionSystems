namespace DecisionSystems.DataPrediction
{
    public class SerializableDataPoint
    {
        public double IndependentValue { get; set; }
        public double DependentValue { get; set; }

        public static DataPoint ToDomain(SerializableDataPoint dataPoint)
        {
            return new DataPoint(dataPoint.IndependentValue, dataPoint.DependentValue);
        }
    }
}