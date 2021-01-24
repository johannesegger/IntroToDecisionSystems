using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.TSP.Solver
{
    public partial class GeneticAlgorithmTSPSolverBase : ITSPSolver
    {
        private readonly Random generator = new Random(777);
        private readonly int populationSize;
        private readonly int iterations;

        public GeneticAlgorithmTSPSolverBase(int populationSize, int iterations)
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
            var population = GenerateRandomPopulation(cities, out var overallBestIndividual);
            for (int i = 0; i < iterations; i++)
            {
                var nextGeneration = new Individual[populationSize];
                for (int j = 0; j < populationSize; j++)
                {
                    var parent1 = TournamentSelect(population, tournamentSize: 3);
                    var parent2 = TournamentSelect(population, tournamentSize: 3);
                    var childTour = OrderCrossover(parent1.Tour, parent2.Tour);
                    TwoOptChange(childTour, mutationProbability: 0.05);
                    var child = new Individual(childTour, cities.GetDistance(childTour));
                    if (child.Fitness < overallBestIndividual.Fitness)
                    {
                        overallBestIndividual = child;
                    }
                    nextGeneration[j] = child;
                }
                population = nextGeneration;
            }
            return overallBestIndividual.Tour.ToList();
        }

        private Individual[] GenerateRandomPopulation(IReadOnlyList<Location> cities, out Individual bestIndividual)
        {
            var population = new Individual[populationSize];
            bestIndividual = default;
            for (int i = 0; i < populationSize; i++)
            {
                var tour = Enumerable.Range(1, cities.Count).Shuffle(generator).ToArray();
                var fitness = cities.GetDistance(tour);
                var individual = new Individual(tour, fitness);
                if (bestIndividual == null || individual.Fitness < bestIndividual.Fitness)
                {
                    bestIndividual = individual;
                }
                population[i] = individual;
            }
            return population;
        }

        private Individual TournamentSelect(IReadOnlyList<Individual> population, int tournamentSize)
        {
            return population.Shuffle(generator).Take(tournamentSize).MinBy(v => v.Fitness);
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
            var parent1SubTour = parent1Tour[startIndex..endIndex];
            var parent2RemainingTour = parent2Tour.Except(parent1SubTour).ToArray();
            var parent2SubTour1 = parent2RemainingTour[..startIndex];
            var parent2SubTour2 = parent2RemainingTour[startIndex..];
            var childTour = new int[parent1Tour.Length];
            parent2SubTour1.CopyTo(childTour, 0);
            parent1SubTour.CopyTo(childTour, parent2SubTour1.Length);
            parent2SubTour2.CopyTo(childTour, parent2SubTour1.Length + parent1SubTour.Length);
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