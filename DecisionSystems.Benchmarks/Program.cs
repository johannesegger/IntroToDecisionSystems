using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DecisionSystems.TSP;
using DecisionSystems.TSP.Solver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DecisionSystems.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<GeneticAlgorithmTSPSolverBenchmarks>();
        }
    }

    [MemoryDiagnoser]
    public class GeneticAlgorithmTSPSolverBenchmarks
    {
        private TSPSpec spec;

        [GlobalSetup]
        public async Task Setup()
        {
            var specs = await new TSPSpecService().GetSpecs();
            spec = specs.Select(SerializableTSPSpec.ToDomain).ElementAt(0);
        }

        [Benchmark(Baseline = true)]
        public string RunBaseSolver()
        {
            return string.Join(", ", new GeneticAlgorithmTSPSolverBase(100, 100).Solve(spec.Cities));
        }

        [Benchmark]
        public string RunImprovedSolver()
        {
            return string.Join(", ", new GeneticAlgorithmTSPSolverImprovedAllocations(100, 100).Solve(spec.Cities));
        }

        [Benchmark]
        public string RunParallelSolver()
        {
            return string.Join(", ", new GeneticAlgorithmTSPSolverParallel(100, 100).Solve(spec.Cities));
        }
    }
}
