using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.TSP.Solver
{
    public class RandomTSPSolver : ITSPSolver
    {
        private readonly int iterations;

        public RandomTSPSolver(int iterations)
        {
            this.iterations = iterations;
        }

        public List<int> Solve(IReadOnlyList<Location> cities)
        {
            return Enumerable.Repeat(0, iterations)
                .Select(_ =>
                    Enumerable.Range(1, cities.Count)
                        .Shuffle()
                        .ToList()
                )
                .MinBy(cities.GetDistance);
        }
    }
}