using System;
using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.TSP
{
    public static class Utils
    {
        public static double GetDistance(Location l1, Location l2)
        {
            var dx = l2.X - l1.X;
            var dy = l2.Y - l1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetDistance(this IReadOnlyList<Location> cities, int idx1, int idx2)
        {
            return GetDistance(cities[idx1 - 1], cities[idx2 - 1]);
        }

        public static double GetDistance(this IReadOnlyList<Location> cities, IReadOnlyCollection<int> solution)
        {
            return solution
                .Concat(new[] { solution.First() })
                .Pairwise(cities.GetDistance)
                .Sum();
        }
    }
}