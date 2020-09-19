namespace DecisionSystems.DataPrediction
{
    public class DataPoint
    {
        public DataPoint(double independentValue, double dependentValue)
        {
            IndependentValue = independentValue;
            DependentValue = dependentValue;
        }

        public double IndependentValue { get; }
        public double DependentValue { get; }
    }
}