using System;
using System.Collections.Generic;

namespace MRC.RobbieRobot.Genetics
{
	public interface IGeneticProblem<T>
	{
		IEnumerable<T> GenerateInitialPopulation(int populationSize);
		double CalculateFitness(T item);
		double GetFitness(T item);
		Tuple<T, T> GetParents(IEnumerable<T> orderedPopulation);
		T ProduceChild(Tuple<T, T> parents);
	}
}