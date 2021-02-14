using DecisionSystems.TSP.Solver;
using System;
using Xunit;

namespace DecisionSystems.Tests
{
    public class GeneticAlgorithmTSPSolverImprovedAllocationsTests
    {
        [Fact]
        public void OrderCrossOver_SectionInTheMiddle()
        {
            var parent1Tour = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var parent2Tour = new[] { 8, 7, 2, 4, 1, 5, 6, 3, 10, 9 };
            var childTour = new int[parent1Tour.Length];
            GeneticAlgorithmTSPSolverImprovedAllocations.OrderCrossover(
                childTour,
                parent1Tour,
                parent2Tour,
                startIndex: 1,
                endIndex: 4
            );
            Assert.Equal("8, 2, 3, 4, 7, 1, 5, 6, 10, 9", string.Join(", ", childTour));
        }
    }
}
