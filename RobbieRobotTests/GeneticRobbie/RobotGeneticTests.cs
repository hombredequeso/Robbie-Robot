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
		public void Can_Run_Robots_Through_Genetic_Processor()
		{
			RobotGeneticProblem robotGeneticProblem = new RobotGeneticProblem();
			GeneticAlgorithmProcessor<Robot> processor = new GeneticAlgorithmProcessor<Robot>(robotGeneticProblem);

			const int populationSize = 500;
			var population = processor.GetInitialPopulation(populationSize).ToArray();
			var initialFitness = processor.CalculatePopulationFitness(population);
			Debug.Print("Initial Fitness: {0}", initialFitness);

			for (int i = 0; i < 1000; i++)
			{
				population = processor.GetNextPopulation(population).ToArray();
				var nextFitness = processor.CalculatePopulationFitness(population);
				Debug.Print("Iteration {0}: {1}", i, nextFitness);
			}
		}
	}

	public class RobotGeneticProblem : IGeneticProblem<Robot>
	{
		private static Random _random;

		static RobotGeneticProblem()
		{
			_random = new Random();
		}

		public IEnumerable<Robot> GenerateInitialPopulation(int populationSize)
		{
			return new Robot[populationSize]
				.Select((x, i) => new Robot(StrategyGenerator.Random(), new Scorer()));
		}

		public double CalculateFitness(Robot robot)
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
			return robot._scorer.Score;
		}

		public double GetFitness(Robot item)
		{
			return item._scorer.Score;
		}

		public Tuple<Robot, Robot> GetParents(IEnumerable<Robot> orderedPopulation)
		{
			var populationSize = orderedPopulation.Count();
			var index1 = GetParentIndex(populationSize);
			var index2 = GetParentIndex(populationSize);
			var parent1 = orderedPopulation.ElementAt(index1);
			var parent2 = orderedPopulation.ElementAt(index2);
			return new Tuple<Robot, Robot>(parent1, parent2);
		}

		public int GetParentIndex(int max)
		{
			return (int)Math.Floor(max * Math.Sqrt(_random.NextDouble()));
		}



		public Robot ProduceChild(Tuple<Robot, Robot> parents)
		{
			var strategy1 = parents.Item1._moveStrategy;
			var strategy2 = parents.Item2._moveStrategy;
			var newStrategy = StrategyGenerator.Merge(strategy1, strategy2);
			return new Robot(newStrategy, new Scorer());
		}
	}
}