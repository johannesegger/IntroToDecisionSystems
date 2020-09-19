using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DecisionSystems.DataPrediction
{
    public class DataPredictionSpecService
    {
        public async Task<SerializableDataPredictionSpec[]> GetSpecs()
        {
            return new[] { await ReadX01(), await ReadX06(), await ReadWeather(), await ReadFish() };
        }

        private async Task<SerializableDataPredictionSpec> ReadX01()
        {
            var lines = await File.ReadAllLinesAsync(@"DataPrediction\data\x01.txt");
            var dataPoints = lines
                .SkipWhile(line => line != "Body Weight")
                .Skip(1)
                .TakeWhile(line => line != "")
                .Select(line =>
                {
                    var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    return new SerializableDataPoint { IndependentValue = double.Parse(parts[1], CultureInfo.InvariantCulture), DependentValue = double.Parse(parts[2], CultureInfo.InvariantCulture) };
                })
                .ToArray();
            var (trainData, testData) = SplitDataPoints(dataPoints, 0.8);
            return new SerializableDataPredictionSpec { Name = "Brain weight -> Body weight", TrainData = trainData, TestData = testData };
        }

        private async Task<SerializableDataPredictionSpec> ReadX06()
        {
            var lines = await File.ReadAllLinesAsync(@"DataPrediction\data\x06.txt");
            var dataPoints = lines
                .SkipWhile(line => line != "Length of fish")
                .Skip(1)
                .TakeWhile(line => line != "")
                .Select(line =>
                {
                    var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    return new SerializableDataPoint { IndependentValue = double.Parse(parts[1], CultureInfo.InvariantCulture), DependentValue = double.Parse(parts[3], CultureInfo.InvariantCulture) };
                })
                .ToArray();
            var (trainData, testData) = SplitDataPoints(dataPoints, 0.8);
            return new SerializableDataPredictionSpec { Name = "Age of fish -> Length of fish", TrainData = trainData, TestData = testData };
        }

        private async Task<SerializableDataPredictionSpec> ReadWeather()
        {
            var lines = await File.ReadAllLinesAsync(@"DataPrediction\data\weatherHistory.csv");
            var dataPoints = lines
                .Skip(1)
                .Select(line =>
                {
                    var parts = line.Split(",", StringSplitOptions.None);
                    return new SerializableDataPoint { IndependentValue = double.Parse(parts[5], CultureInfo.InvariantCulture), DependentValue = double.Parse(parts[3], CultureInfo.InvariantCulture) };
                })
                .ToArray();
            var (trainData, testData) = SplitDataPoints(dataPoints, 0.8);
            return new SerializableDataPredictionSpec { Name = "Humidity -> Temperature", TrainData = trainData, TestData = testData };
        }

        private async Task<SerializableDataPredictionSpec> ReadFish()
        {
            var lines = await File.ReadAllLinesAsync(@"DataPrediction\data\Fish.csv");
            var dataPoints = lines
                .Skip(1)
                .Select(line =>
                {
                    var parts = line.Split(",", StringSplitOptions.None);
                    return new SerializableDataPoint { IndependentValue = double.Parse(parts[1], CultureInfo.InvariantCulture), DependentValue = double.Parse(parts[2], CultureInfo.InvariantCulture) };
                })
                .ToArray();
            var (trainData, testData) = SplitDataPoints(dataPoints, 0.8);
            return new SerializableDataPredictionSpec { Name = "Weight of fish -> Length of fish", TrainData = trainData, TestData = testData };
        }

        private static Random generator = new Random();
        private static (SerializableDataPoint[], SerializableDataPoint[]) SplitDataPoints(IReadOnlyCollection<SerializableDataPoint> dataPoints, double threshold)
        {
            var shuffledDataPoints = dataPoints
                .OrderBy(_ => generator.NextDouble())
                .ToArray();
            var separatorIndex = (int)Math.Round(dataPoints.Count * threshold);
            var trainData = shuffledDataPoints[0..separatorIndex];
            var testData = shuffledDataPoints[separatorIndex..];
            return (trainData, testData);
        }
    }
}