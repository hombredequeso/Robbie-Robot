using System.Collections.Generic;
using System.Linq;

namespace MRC.RobbieRobot.Genetics
{
	public class GeneticAlgorithmProcessor<T>
	{
		public GeneticAlgorithmProcessor(IGeneticProblem<T> problem)
		{
			_problem = problem;
		}

		private readonly IGeneticProblem<T> _problem;

		public IEnumerable<T> GetInitialPopulation(int populationSize)
		{
			return _problem.GenerateInitialPopulation(populationSize);
		}

		public IEnumerable<T> GetNextPopulation(IEnumerable<T> initialPopulation)
		{
			var orderedPopulation = initialPopulation
				.OrderBy(x => _problem.GetFitness(x))
				.ToArray();
			var result = orderedPopulation.AsParallel()
				.Select(x => _problem.GetParents(orderedPopulation))
				.Select(x => _problem.ProduceChild(x));
			return result;
		}

		public double CalculatePopulationFitness(IEnumerable<T> population)
		{
			var popAsPar = population.AsParallel();
			popAsPar.ForAll(x => _problem.CalculateFitness(x));
			return popAsPar.Select(x => _problem.GetFitness(x)).Average();
		}
	}
}