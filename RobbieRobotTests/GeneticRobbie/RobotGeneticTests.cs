using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRC.RobbieRobot.Genetics;
using MRC.RobbieRobot.RobbieGame;
using NUnit.Framework;

namespace MRC.RobbieRobotTests.GeneticRobbie
{
	[TestFixture]
	public class RobotGeneticTests
	{
		[Test]
		public void Can_Run_A_Robot_Through_A_Game_And_Get_A_Score()
		{
			IDictionary<Situation, RobotAction> moveStrategy = StrategyGenerator.Random();
			Scorer scorer = new Scorer();
			Robot robot = new Robot(moveStrategy, scorer);
			Board board = new Board(10, 10);

			var litterer = new BoardLitterer(0.5);
			litterer.Litter(board);


			board.AddElement(robot, new Point(0, 0));

			const int numberOfTurns = 200;
			for (int i = 0; i < numberOfTurns; i++)
			{
				robot.Act(board);
			}

			int score = robot._scorer.Score;
			Debug.Print("Score is: {0}", score);
		}

		[Test]
		public void Can_Combine_Strategies()
		{
			IDictionary<Situation, RobotAction> s1 = StrategyGenerator.Random();
			IDictionary<Situation, RobotAction> s2 = StrategyGenerator.Random();

			var newStrategy = StrategyGenerator.Merge(s1, s2);

			Assert.AreEqual(s1.Count, newStrategy.Count);
			Assert.AreEqual(s2.Count, newStrategy.Count);

		}

		[Test]
		public void Can_Test_Multiple_Strategies_And_Order_Them_By_Score()
		{
			int strategiesToTest = 10;
			var strategies = new Dictionary<Situation, RobotAction>[strategiesToTest]
				.Select(x => StrategyGenerator.Random()).ToArray();

			var testingRobots = new Robot[strategiesToTest]
				.Select((x, i) => new Robot(strategies[i], new Scorer())).ToArray();

			foreach (var robot in testingRobots)
			{
				Board b = new Board(10, 10);
				var litterer = new BoardLitterer(0.5);
				litterer.Litter(b);
				b.AddElement(robot, new Point(0, 0));

				const int numberOfTurns = 200;
				for (int i = 0; i < numberOfTurns; i++)
				{
					robot.Act(b);
				}
			}

			var orderedResults = testingRobots.OrderBy(x => x._scorer.Score);

			Debug.Print(String.Join(", ", orderedResults.Select(x => x._scorer.Score)));
		}

		[Test]
		public void GetParentIndex_IsEvenly_Distributed()
		{
			int populationSize = 10;
			int[] indexSelectionCount = new int[populationSize * 3];
			for (int i = 0; i < 10000; i++)
				indexSelectionCount[GenerateRobotsByPairing.GetParentIndex(populationSize)]++;
			for (int c = 0; c < populationSize * 3; c++)
			{
				Debug.WriteLine("{0}: {1}", c, indexSelectionCount[c]);
			}
		}


		[Test]
		public void Can_Run_Robots_Through_Genetic_Processor()
		{
			RobotGeneticProblem robotGeneticProblem = new RobotGeneticProblem();
			// IChildGenerator<Robot> childGenerator = new GenerateRobotsByPairing();
			IChildGenerator<Robot> childGenerator = new GenerateRobotsByMutation();
			GeneticAlgorithmProcessor<Robot> processor = new GeneticAlgorithmProcessor<Robot>(robotGeneticProblem, childGenerator);

			const int populationSize = 300;
			var population = processor.GetInitialPopulation(populationSize).ToArray();
			var initialFitness = processor.CalculatePopulationFitness(population);
			Debug.Print("Initial Fitness: {0}", initialFitness);

			for (int i = 0; i < 500; i++)
			{
				population = processor.GetNextPopulation(population).ToArray();
				var nextFitness = processor.CalculatePopulationFitness(population);
				var maxFitness = population.Max(x => robotGeneticProblem.GetFitness(x));

				Debug.Print("Iteration {0}: avg = {1}, max = {2}", i, nextFitness, maxFitness);
			}
		}
	}

	public class RobotGeneticProblem : IGeneticProblem<Robot>
	{
		public IEnumerable<Robot> GenerateInitialPopulation(int populationSize)
		{
			return new Robot[populationSize]
				.Select((x, i) => new Robot(StrategyGenerator.Random(), new Scorer()));
		}

		private const int BoardCountToCalculateFitness = 15;

		public double CalculateFitness(Robot robot)
		{
			for (int boardsToTest = 0; boardsToTest < BoardCountToCalculateFitness; boardsToTest++)
			{
				Board b = new Board(10, 10);
				var litterer = new BoardLitterer(0.5);
				litterer.Litter(b);
				b.AddElement(robot, new Point(0, 0));

				const int numberOfTurns = 200;
				for (int i = 0; i < numberOfTurns; i++)
				{
					robot.Act(b);
				}
			}
			return (double)robot._scorer.Score / BoardCountToCalculateFitness;
		}

		public double GetFitness(Robot item)
		{
			return (double)item._scorer.Score / BoardCountToCalculateFitness;
		}
	}

	public class GenerateRobotsByPairing : GenerateByPairing<Robot>
	{
		private static readonly Random _random;

		static GenerateRobotsByPairing()
		{
			_random = new Random();
		}

		protected override Tuple<Robot, Robot> GetParents(Robot[] orderedPopulation)
		{
			var populationSize = orderedPopulation.Count();
			var index1 = GetParentIndex(populationSize);
			var index2 = GetParentIndex(populationSize);
			var parent1 = orderedPopulation[index1];
			var parent2 = orderedPopulation[index2];
			return new Tuple<Robot, Robot>(parent1, parent2);
		}

		public static int GetParentIndex(int max)
		{
			//return (int)Math.Floor(max * Math.Sqrt(_random.NextDouble()));
			//return  (int)Math.Floor(Math.Sqrt(_random.NextDouble() * 3)/Math.Sqrt(3)*max);
			//return (int) Math.Floor(Math.Pow(3.0*_random.NextDouble(), 1.0/3)/Math.Pow(3.0, 1.0/3)*max);
			return (int)Math.Floor(Math.Pow(4.0 * _random.NextDouble(), 1.0 / 4) / Math.Pow(4.0, 1.0 / 4) * max);
		}


		protected override Robot ProduceChild(Tuple<Robot, Robot> parents)
		{
			var strategy1 = parents.Item1._moveStrategy;
			var strategy2 = parents.Item2._moveStrategy;
			var newStrategy = StrategyGenerator.Merge(strategy1, strategy2);
			return new Robot(newStrategy, new Scorer());
		}
	}

	public class GenerateRobotsByMutation: IChildGenerator<Robot>
	{
		public Robot GenerateChild(Robot[] orderedPopulation)
		{
			var populationSize = orderedPopulation.Count();
			var parent = orderedPopulation[GetParentIndex(populationSize)];
			var newStrategy = new Dictionary<Situation, RobotAction>(parent._moveStrategy);
			StrategyGenerator.RandomlyChange(0, 20, newStrategy);
			return new Robot(newStrategy, new Scorer());
		}

		private static readonly Random _random;

		static GenerateRobotsByMutation()
		{
			_random = new Random();
		}


		public static int GetParentIndex(int max)
		{
			//return (int)Math.Floor(max * Math.Sqrt(_random.NextDouble()));
			//return  (int)Math.Floor(Math.Sqrt(_random.NextDouble() * 3)/Math.Sqrt(3)*max);
			//return (int) Math.Floor(Math.Pow(3.0*_random.NextDouble(), 1.0/3)/Math.Pow(3.0, 1.0/3)*max);
			return (int)Math.Floor(Math.Pow(4.0 * _random.NextDouble(), 1.0 / 4) / Math.Pow(4.0, 1.0 / 4) * max);
		}

	}
}