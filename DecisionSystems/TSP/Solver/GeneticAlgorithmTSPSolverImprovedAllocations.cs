using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.TSP.Solver
{
    public partial class GeneticAlgorithmTSPSolverImprovedAllocations : ITSPSolver
    {
        private readonly Random generator = new Random(777);
        private readonly int populationSize;
        private readonly int iterations;

        public GeneticAlgorithmTSPSolverImprovedAllocations(int populationSize, int iterations)
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
            var parentPopulation = GenerateRandomPopulation(cities, out var parentTours, out var overallBestIndividual);
            var childPopulation = new Individual[populationSize];
            var childTours = new int[populationSize * cities.Count];
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < populationSize; j++)
                {
                    var parent1 = TournamentSelect(parentPopulation);
                    var parent2 = TournamentSelect(parentPopulation);
                    var childTour = new ArraySegment<int>(childTours, j * cities.Count, cities.Count);
                    OrderCrossover(childTour, parent1.Tour, parent2.Tour);
                    TwoOptChange(childTour, mutationProbability: 0.05);
                    var child = new Individual(childTour, cities.GetDistance(childTour));
                    if (child.Fitness < overallBestIndividual.Fitness)
                    {
                        overallBestIndividual = child;
                    }
                    childPopulation[j] = child;
                }
                (parentPopulation, childPopulation) = (childPopulation, parentPopulation);
                (parentTours, childTours) = (childTours, parentTours);
            }
            return overallBestIndividual.Tour.ToList();
        }

        private Individual[] GenerateRandomPopulation(IReadOnlyList<Location> cities, out int[] tours, out Individual bestIndividual)
        {
            var population = new Individual[populationSize];
            tours = new int[populationSize * cities.Count];
            bestIndividual = default;
            for (int i = 0; i < populationSize; i++)
            {
                var tour = new ArraySegment<int>(tours, i * cities.Count, cities.Count);
                Enumerable.Range(1, cities.Count).Shuffle(generator).CopyTo(tour.AsSpan());
                var fitness = cities.GetDistance(tour);
                var individual = new Individual(tour, fitness);
                if (i == 0 || individual.Fitness < bestIndividual.Fitness)
                {
                    bestIndividual = individual;
                }
                population[i] = individual;
            }
            return population;
        }

        private Individual TournamentSelect(IReadOnlyList<Individual> population)
        {
            var individual1Idx = generator.Next(population.Count);
            var winnerIdx = individual1Idx;

            int individual2Idx;
            do
            {
                individual2Idx = generator.Next(population.Count);
            }
            while (individual2Idx == individual1Idx);
            if (population[individual2Idx].Fitness < population[winnerIdx].Fitness)
            {
                winnerIdx = individual2Idx;
            }

            int individual3Idx;
            do
            {
                individual3Idx = generator.Next(population.Count);
            }
            while (individual3Idx == individual2Idx || individual3Idx == individual1Idx);
            if (population[individual3Idx].Fitness < population[winnerIdx].Fitness)
            {
                winnerIdx = individual3Idx;
            }
            return population[winnerIdx];
        }

        private void OrderCrossover(ArraySegment<int> childTour, ArraySegment<int> parent1Tour, ArraySegment<int> parent2Tour)
        {
            var cityCount = parent1Tour.Count;
            var startIndex = generator.Next(cityCount);
            var endIndex = generator.Next(cityCount);
            OrderCrossover(childTour, parent1Tour, parent2Tour, startIndex, endIndex);
        }

        public static void OrderCrossover(ArraySegment<int> childTour, ArraySegment<int> parent1Tour, ArraySegment<int> parent2Tour, int startIndex, int endIndex)
        {
            if (startIndex > endIndex)
            {
                (startIndex, endIndex) = (endIndex, startIndex);
                (parent1Tour, parent2Tour) = (parent2Tour, parent1Tour);
            }
            var cityCount = parent1Tour.Count;
            var parent1SubTour = parent1Tour.Slice(startIndex, endIndex - startIndex);
            var parent1SubTourMask = ArrayPool<bool>.Shared.Rent(cityCount);
            for (int i = 0; i < parent1SubTourMask.Length; i++)
            {
                parent1SubTourMask[i] = false;
            }
            for (int i = 0; i < parent1SubTour.Count; i++)
            {
                parent1SubTourMask[parent1SubTour[i] - 1] = true;
            }
            var parent2RemainingTourLength = parent2Tour.Count - parent1SubTour.Count;
            var parent2RemainingTour = ArrayPool<int>.Shared.Rent(parent2RemainingTourLength);
            int j = 0;
            for (int i = 0; i < parent2Tour.Count; i++)
            {
                if (!parent1SubTourMask[parent2Tour[i] - 1])
                {
                    parent2RemainingTour[j++] = parent2Tour[i];
                }
            }
            var parent2SubTour1 = new ArraySegment<int>(parent2RemainingTour, 0, startIndex);
            var parent2SubTour2 = new ArraySegment<int>(parent2RemainingTour, startIndex, parent2RemainingTourLength - startIndex);
            ArrayPool<bool>.Shared.Return(parent1SubTourMask);
            ArrayPool<int>.Shared.Return(parent2RemainingTour);
            parent2SubTour1.CopyTo(childTour);
            parent1SubTour.CopyTo(childTour.Slice(parent2SubTour1.Count));
            parent2SubTour2.CopyTo(childTour.Slice(parent2SubTour1.Count + parent1SubTour.Count));
        }

        private void TwoOptChange(ArraySegment<int> child, double mutationProbability)
        {
            if (generator.NextDouble() >= mutationProbability)
            {
                return;
            }
            int index1 = generator.Next(child.Count);
            int index2 = (index1 + 1) % child.Count;
            child.Swap(index1, index2);
        }

        private struct Individual
        {
            public Individual(ArraySegment<int> tour, double fitness)
            {
                Tour = tour;
                Fitness = fitness;
            }

            public ArraySegment<int> Tour { get; }
            public double Fitness { get; }
        }
    }
}