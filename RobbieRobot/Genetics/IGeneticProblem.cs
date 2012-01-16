using System;
using System.Collections.Generic;

namespace MRC.RobbieRobot.Genetics
{
	public interface IGeneticProblem<T>
	{
		IEnumerable<T> GenerateInitialPopulation(int populationSize);
		double CalculateFitness(T item);
		double GetFitness(T item);
		//Tuple<T, T> GetParents(IEnumerable<T> orderedPopulation);
		//T ProduceChild(Tuple<T, T> parents);
	}

	public interface IChildGenerator<T>
	{
		T GenerateChild(T[] orderedPopulation);
	}

	public abstract class GenerateByPairing<T>: IChildGenerator<T>
	{
		public T GenerateChild(T[] orderedPopulation)
		{
			var parents = GetParents(orderedPopulation);
			var child = ProduceChild(parents);
			return child;
		}

		protected abstract Tuple<T, T> GetParents(T[] orderedPopulation);
		protected abstract T ProduceChild(Tuple<T, T> parents);
	}
}