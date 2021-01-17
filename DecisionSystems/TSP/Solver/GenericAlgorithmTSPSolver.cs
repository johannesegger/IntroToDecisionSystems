using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DecisionSystems.TSP.Solver
{
    public partial class GenericAlgorithmTSPSolver : ITSPSolver
    {
        private readonly Random generator = new Random();
        private readonly int populationSize;
        private readonly int iterations;

        public GenericAlgorithmTSPSolver(int populationSize, int iterations)
        {
            if (populationSize <= 0)
            {
                throw new ArgumentException("Population size must be positive.");
            }
            this.populationSize = populationSize;

            if (iterations <= 0)
            {
                throw new ArgumentException("Number of iterations must be positive.");
            }
            this.iterations = iterations;
        }

        public List<int> Solve(IReadOnlyList<Location> cities)
        {
            var fullPopulation = ArrayPool<Individual>.Shared.Rent(populationSize);
            var population = new ArraySegment<Individual>(fullPopulation, 0, populationSize);
            Individual overallBestIndividual = default;
            for (int i = 0; i < populationSize; i++)
            {
                var tour = Enumerable.Range(1, cities.Count).Shuffle().ToArray();
                var fitness = cities.GetDistance(tour);
                var individual = new Individual(tour, fitness);
                if (overallBestIndividual == null || individual.Fitness < overallBestIndividual.Fitness)
                {
                    overallBestIndividual = individual;
                }
                population[i] = individual;
            }
            Stopwatch totalWatch = Stopwatch.StartNew();
            Stopwatch selectWatch = new Stopwatch();
            Stopwatch crossoverWatch = new Stopwatch();
            Stopwatch mutationWatch = new Stopwatch();
            for (int i = 0; i < iterations; i++)
            {
                var fullNextGeneration = ArrayPool<Individual>.Shared.Rent(populationSize);
                var nextGeneration = new ArraySegment<Individual>(fullNextGeneration, 0, populationSize);
                for (int j = 0; j < populationSize; j++)
                {
                    selectWatch.Start();
                    var parent1 = TournamentSelect(population, tournamentSize: 3);
                    var parent2 = TournamentSelect(population, tournamentSize: 3);
                    selectWatch.Stop();
                    crossoverWatch.Start();
                    var childTour = OrderCrossover(parent1.Tour, parent2.Tour);
                    crossoverWatch.Stop();
                    mutationWatch.Start();
                    TwoOptChange(childTour, mutationProbability: 0.05);
                    mutationWatch.Stop();
                    var child = new Individual(childTour, cities.GetDistance(childTour));
                    if (child.Fitness < overallBestIndividual.Fitness)
                    {
                        overallBestIndividual = child;
                    }
                    nextGeneration[j] = child;
                }
                ArrayPool<Individual>.Shared.Return(fullPopulation);
                fullPopulation = fullNextGeneration;
            }
            totalWatch.Stop();
            Debug.WriteLine($"Selection time: {selectWatch.Elapsed}");
            Debug.WriteLine($"Crossover time: {crossoverWatch.Elapsed}");
            Debug.WriteLine($"Mutation time: {mutationWatch.Elapsed}");
            Debug.WriteLine($"Total time: {totalWatch.Elapsed}");
            return overallBestIndividual.Tour.ToList();
        }

        private Individual TournamentSelect(IReadOnlyList<Individual> population, int tournamentSize)
        {
            Individual winner = population[generator.Next(population.Count)];
            var participants = new HashSet<Individual>(tournamentSize) { winner };
            while (participants.Count < tournamentSize)
            {
                var participant = population[generator.Next(population.Count)];
                if (participants.Add(participant) && participant.Fitness < winner.Fitness)
                {
                    winner = participant;
                }
            }
            return winner;
        }

        private int[] OrderCrossover(int[] parent1Tour, int[] parent2Tour)
        {
            var startIndex = generator.Next(parent1Tour.Length);
            var endIndex = generator.Next(parent1Tour.Length);
            if (startIndex > endIndex)
            {
                (startIndex, endIndex) = (endIndex, startIndex);
                (parent1Tour, parent2Tour) = (parent2Tour, parent1Tour);
            }
            var parent1SubTour = new ArraySegment<int>(parent1Tour, startIndex, endIndex - startIndex);
            var parent2RemainingTour = parent2Tour.Except(parent1SubTour).ToArray();
            var parent2SubTour1 = new ArraySegment<int>(parent2RemainingTour, 0, startIndex);
            var parent2SubTour2 = new ArraySegment<int>(parent2RemainingTour, startIndex, parent2RemainingTour.Length - startIndex);
            var childTour = new int[parent1Tour.Length];
            parent2SubTour1.CopyTo(childTour);
            parent1SubTour.CopyTo(childTour, parent2SubTour1.Count);
            parent2SubTour2.CopyTo(childTour, parent2SubTour1.Count + parent1SubTour.Count);
            return childTour;
        }

        private void TwoOptChange(int[] child, double mutationProbability)
        {
            if (generator.NextDouble() >= mutationProbability)
            {
                return;
            }
            int index1 = generator.Next(child.Length);
            int index2 = (index1 + 1) % child.Length;
            child.Swap(index1, index2);
        }

        private class Individual
        {
            public Individual(int[] tour, double fitness)
            {
                Tour = tour;
                Fitness = fitness;
            }

            public int[] Tour { get; }
            public double Fitness { get; }
        }
    }
}