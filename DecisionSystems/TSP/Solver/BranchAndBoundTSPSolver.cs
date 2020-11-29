using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems.TSP.Solver
{
    public class BranchAndBoundTSPSolver : ITSPSolver
    {
        public List<int> Solve(IReadOnlyList<Location> cities)
        {
            var baseTour = Enumerable.Range(1, cities.Count).ToArray();
            List<int[]> tours = new List<int[]>();
            double bestDistance = double.MaxValue;
            void CalculatePermutations(
                int[] data,
                int startIndex,
                double currentDistance)
            {
                if (startIndex == data.Length)
                {
                    var lastSegment = cities.GetDistance(data[startIndex - 1], data[0]);
                    var totalTourDistance = currentDistance + lastSegment;
                    if (totalTourDistance < bestDistance)
                    {
                        bestDistance = totalTourDistance;
                        tours.Add(data.ToArray());
                    }
                }
                else
                {
                    // Calculate permutations by placing permutation[startIndex] at every possible index
                    // and then calculate permutations of elements with index > startIndex
                    for (int i = startIndex; i < data.Length; i++)
                    {
                        data.Swap(startIndex, i);
                        var nextSegmentDistance = cities.GetDistance(data[startIndex - 1], data[startIndex]);
                        var remainingTourDistance = cities.GetDistance(data[startIndex], data[0]);

                        // Only continue if current tour has a chance to become the best solution
                        if (currentDistance + nextSegmentDistance + remainingTourDistance < bestDistance)
                        {
                            CalculatePermutations(
                                data,
                                startIndex + 1,
                                currentDistance + nextSegmentDistance);
                        }
                        data.Swap(startIndex, i);
                    }
                }
            }
            CalculatePermutations(baseTour, 1, 0);
            return tours.MinBy(cities.GetDistance).ToList();
        }

    }
}
