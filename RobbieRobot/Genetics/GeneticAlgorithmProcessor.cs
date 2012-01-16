using System.Collections.Generic;
using System.Linq;

namespace MRC.RobbieRobot.Genetics
{
	public class GeneticAlgorithmProcessor<T>
	{
		public GeneticAlgorithmProcessor(IGeneticProblem<T> problem, IChildGenerator<T> childGenerator)
		{
			_problem = problem;
			_childGenerator = childGenerator;
		}

		private readonly IGeneticProblem<T> _problem;
		private readonly IChildGenerator<T> _childGenerator;

		public IEnumerable<T> GetInitialPopulation(int populationSize)
		{
			return _problem.GenerateInitialPopulation(populationSize);
		}

		public IEnumerable<T> GetNextPopulation(IEnumerable<T> initialPopulation)
		{
			var orderedPopulation = initialPopulation
				.OrderBy(x => _problem.GetFitness(x))
				.ToArray();

			var result = orderedPopulation
				.AsParallel()
				.Select(x => _childGenerator.GenerateChild(orderedPopulation));
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