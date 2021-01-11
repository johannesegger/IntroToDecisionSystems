using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.TSP.Solver
{
    public class NearestNeighborConstructionWithOptimalStartTSPSolver : ITSPSolver
    {
        public List<int> Solve(IReadOnlyList<Location> cities)
        {
            List<int> Solve(int startCity)
            {
                var result = Enumerable.Range(1, cities.Count).ToList();
                result.Swap(0, startCity - 1);
                for (var i = 1; i < result.Count; i++)
                {
                    var minDistance = cities.GetDistance(result[i - 1], result[i]);
                    var next = i;
                    for (var j = i + 1; j < result.Count; j++)
                    {
                        var distance = cities.GetDistance(result[i - 1], result[j]);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            next = j;
                        }
                    }
                    result.Swap(i, next);
                }
                return result;
            }

            var bestTour = Solve(1);
            var minDistance = cities.GetDistance(bestTour);
            for (int i = 2; i < cities.Count; i++)
            {
                var tour = Solve(i);
                var distance = cities.GetDistance(tour);
                if (distance < minDistance)
                {
                    bestTour = tour;
                    minDistance = distance;
                }
            }
            return bestTour;
        }
    }
}