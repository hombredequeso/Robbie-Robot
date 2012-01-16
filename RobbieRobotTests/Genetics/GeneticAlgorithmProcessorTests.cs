using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRC.RobbieRobot.Genetics;
using NUnit.Framework;

namespace MRC.RobbieRobotTests.Genetics
{
	[TestFixture]
	public class GeneticAlgorithmProcessorTests
	{
		[Test]
		public void Processor_Can_Generate_Initial_Population()
		{
			IGeneticProblem<int> problem = new TestProblem();
			IChildGenerator<int> childGenerator = new TestChildGenerator();
			GeneticAlgorithmProcessor<int> processor = new GeneticAlgorithmProcessor<int>(problem, childGenerator);
			int populationSize = 100;
			var initialPopulation = processor.GetInitialPopulation(populationSize);

			Assert.AreEqual(populationSize, initialPopulation.Count());
		}

		[Test]
		public void Processor_Can_Generate_Next_Generation()
		{
			IGeneticProblem<int> problem = new TestProblem();
			IChildGenerator<int> childGenerator = new TestChildGenerator();
			GeneticAlgorithmProcessor<int> processor = new GeneticAlgorithmProcessor<int>(problem, childGenerator);
			int populationSize = 100;
			var initialPopulation = processor.GetInitialPopulation(populationSize);
			var nextGeneration = processor.GetNextPopulation(initialPopulation);

			Assert.AreEqual(populationSize, nextGeneration.Count());
		}

		[Test]
		public void Processor_Can_Calculate_Population_Fitness()
		{
			IGeneticProblem<int> problem = new TestProblem();
			IChildGenerator<int> childGenerator = new TestChildGenerator();
			GeneticAlgorithmProcessor<int> processor = new GeneticAlgorithmProcessor<int>(problem, childGenerator);
			int populationSize = 100;
			var initialPopulation = processor.GetInitialPopulation(populationSize);
			var populationFitness = processor.CalculatePopulationFitness(initialPopulation);
			Assert.IsTrue(populationFitness != 0, "Shouldn't happen, or at least, won't normally happen");
		}

		[Test]
		public void Processor_Can_Improve_On_Each_Generation()
		{
			IGeneticProblem<int> problem = new TestProblem();
			IChildGenerator<int> childGenerator = new TestChildGenerator();
			GeneticAlgorithmProcessor<int> processor = new GeneticAlgorithmProcessor<int>(problem, childGenerator);
			const int populationSize = 100;
			var initialPopulation = processor.GetInitialPopulation(populationSize).ToArray();
			var initialFitness = processor.CalculatePopulationFitness(initialPopulation);

			var nextPopulation = processor.GetNextPopulation(initialPopulation);
			var nextFitness = processor.CalculatePopulationFitness(nextPopulation);
			Assert.IsTrue(nextFitness < initialFitness, "Fitness moved from {0} to {1}", initialFitness, nextFitness);
		}

		[Test]
		public void Processor_Can_Converge_Towards_A_Solution()
		{
			IGeneticProblem<int> problem = new TestProblem();
			IChildGenerator<int> childGenerator = new TestChildGenerator();
			GeneticAlgorithmProcessor<int> processor = new GeneticAlgorithmProcessor<int>(problem, childGenerator);

			const int populationSize = 10000;
			int[] population = processor.GetInitialPopulation(populationSize).ToArray();
			var initialFitness = processor.CalculatePopulationFitness(population);
			Debug.Print("Initial Fitness: {0}", initialFitness);

			for (int i = 0; i < 200; i++)
			{
				population = processor.GetNextPopulation(population).ToArray();
				var nextFitness = processor.CalculatePopulationFitness(population);
				Debug.Print("Iteration {0}: {1}", i, nextFitness);
				Assert.IsTrue(nextFitness < initialFitness);
			}
		}

		[Test]
		public void GetIndexFromPopulation_Is_Weighted_Towards_Zero()
		{
			const int populationSize = 100;
			int[] count = new int[populationSize];
			TestChildGenerator childGenerator = new TestChildGenerator();

			for (int i = 0; i < 1000000; i++)
				count[childGenerator.GetParentIndex(populationSize)]++;

			for (int i = 0; i < 100; i++)
				Debug.Print("{0} : {1}", i, count[i]);

			int subsequentLargerThanPrevious = 0;
			int previous = count[0];
			for (int i = 1; i < 100; i++)
			{
				if (count[i] > previous) 
					++subsequentLargerThanPrevious;
				else 
					previous = count[i];
			}

			Debug.Print("Out of Order: {0}", subsequentLargerThanPrevious);
			Assert.IsTrue(subsequentLargerThanPrevious < 15);
		}



	}

	public class TestProblem : IGeneticProblem<int>
	{
		private static Random _random;

		static TestProblem()
		{
			_random = new Random();
		}

		public IEnumerable<int> GenerateInitialPopulation(int populationSize)
		{
			var population = new int[populationSize];
			for (int i = 0; i < populationSize; i++)
			{
				population[i] = _random.Next(1, 100);
			}
			return population;
		}

		public double CalculateFitness(int item)
		{
			return Math.Abs(item - 50);
		}

		public double GetFitness(int item)
		{
			return Math.Abs(item - 50);			
		}
	}

	public class TestChildGenerator: GenerateByPairing<int>
	{
		private static Random _random;

		static TestChildGenerator()
		{
			_random = new Random();
		}

		protected override Tuple<int, int> GetParents(int[] orderedPopulation)
		{
			var populationSize = orderedPopulation.Count();

			int parentIndex = GetParentIndex(populationSize);
			var parent1 = orderedPopulation[parentIndex];

			int index0 = GetParentIndex(populationSize);
			var parent2 = orderedPopulation[index0];

			return new Tuple<int, int>(parent1, parent2);
		}

		protected override int ProduceChild(Tuple<int, int> parents)
		{
			return ((parents.Item1 + parents.Item2) / 2);
		}

		public int GetParentIndex(int max)
		{
			var parentIndex = (int) Math.Floor(max - max*Math.Sqrt(_random.NextDouble()));
			return parentIndex == max ? max - 1: parentIndex;
		}
	}
}