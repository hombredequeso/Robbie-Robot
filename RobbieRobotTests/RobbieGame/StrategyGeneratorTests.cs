using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRC.RobbieRobot.RobbieGame;
using NUnit.Framework;

namespace MRC.RobbieRobotTests.RobbieGame
{
	[TestFixture]
	public class StrategyGeneratorTests
	{
		private void Test_Random_Generator_Returns_Some_Of_Every_Value<T>(Func<T> randomGenerator)
		{
			// Arrange
			var allActions = Enum.GetValues(typeof(T)).Cast<T>().ToList();

			IDictionary<T, int> results = new Dictionary<T, int>();
			allActions.ForEach(x => results.Add(x, 0));

			// Act
			for (int i = 0; i < 100; i++)
			{
				results[randomGenerator()]++;
			}

			// Assert

			foreach (var robotAction in allActions)
			{
				Assert.AreNotEqual(0, results[robotAction], "Random generator never returned {0}", robotAction);
			}
			
		}

		[Test]
		public void GetRandomAction_Returns_Some_Of_Every_Value()
		{
			Test_Random_Generator_Returns_Some_Of_Every_Value(StrategyGenerator.GetRandomAction);
		}

		[Test]
		public void GetRandomSituation_Returns_Some_Of_Every_Value()
		{
			// Arrange
			var allSituations = GetAllSituations().ToList();

			IDictionary<Situation, int> results = new Dictionary<Situation, int>();
			allSituations.ForEach(x => results.Add(x, 0));

			// Act
			for (int i = 0; i < 2000; i++)
			{
				results[StrategyGenerator.GetRandomSituation()]++;
			}

			// Assert

			int total = 0;
			foreach (var robotAction in allSituations)
			{
				total += results[robotAction];
				Debug.WriteLine("{0}: count = {1}", robotAction, results[robotAction]);
			}
			Debug.WriteLine("Total: {0}", total);

			foreach (var robotAction in allSituations)
			{
				Assert.AreNotEqual(0, results[robotAction], "Random generator never returned {0}", robotAction);
			}
		}

		private IEnumerable<Situation> GetAllSituations()
		{
			var contentPossibilities = Enum.GetValues(typeof(Situation.BoardContents)).Cast<Situation.BoardContents>().ToArray();
			foreach (var north in contentPossibilities)
			{
				foreach (var south in contentPossibilities)
				{
					foreach (var east in contentPossibilities)
					{
						foreach (var west in contentPossibilities)
						{
							foreach (var current in contentPossibilities)
							{
								yield return new Situation(current, north, south, east, west);
							}
						}
					}
				}
			}

		}


		[Test]
		public void GetRandomAction_Returns_A_Spread_Of_Actions_Across_All_Available_Actions()
		{
			// Arrange
			var allActions = Enum.GetValues(typeof(RobotAction)).Cast<RobotAction>().ToList();

			IDictionary<RobotAction, int> results = new Dictionary<RobotAction, int>();
			allActions.ForEach(x => results.Add(x, 0));

			// Act
			for (int i = 0; i < 100; i++)
			{
				results[StrategyGenerator.GetRandomAction()]++;
			}

			// Assert

			foreach (var robotAction in allActions)
			{
				Assert.AreNotEqual(0, results[robotAction], "Random generator never returned {0}", robotAction);
			}
		}
	}
}
