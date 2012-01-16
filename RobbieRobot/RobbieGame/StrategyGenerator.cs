using System;
using System.Collections.Generic;
using System.Linq;

namespace MRC.RobbieRobot.RobbieGame
{
	public static class StrategyGenerator
	{
		// private static Random _random;
		private static int _maxAction;
		private static int _contentCount;
		private static int _actionCount;

		[ThreadStatic]
		private static Random _random;

		private static Random random { get { return _random ?? (_random = new Random()); } }


		static StrategyGenerator()
		{
			//_random = new Random();
			_maxAction = Enum.GetValues(typeof (RobotAction)).Cast<RobotAction>().Max(x => (int) x);
			_contentCount = Enum.GetValues(typeof (Situation.BoardContents)).GetLength(0);
			_actionCount = Enum.GetValues(typeof(RobotAction)).GetLength(0);
		}

		public static IDictionary<Situation, RobotAction> Random()
		{
			var strategy = new Dictionary<Situation, RobotAction>();
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
								Situation s = new Situation(current, north, south, east, west);
								RobotAction a = (RobotAction)random.Next(0, _maxAction + 1);
								strategy[s] = a;
							}
						}
					}
				}
			}
			return strategy;
		}

		public static IDictionary<Situation, RobotAction> Merge(IDictionary<Situation, RobotAction> s1, IDictionary<Situation, RobotAction> s2)
		{
			var newStrategy = new Dictionary<Situation, RobotAction>();
			var e = s1.GetEnumerator();
			bool chooseE1 = random.Next(0, 2) == 0;
			while (e.MoveNext())
			{
				RobotAction e1 = e.Current.Value;
				var chosenAction = chooseE1 ? e1 : s2[e.Current.Key];
				newStrategy.Add(e.Current.Key, chosenAction);
				chooseE1 = !chooseE1;
			}
			//RandomlyChange(20, newStrategy);
			RandomlyChange(0,20, newStrategy);
			return newStrategy;
		}

		public static void RandomlyChange(float percentageOfSituations, IDictionary<Situation, RobotAction> strategy)
		{
			int itemsToChange = (int) (strategy.Count()*percentageOfSituations/100.0);
			for (int i = 0; i < itemsToChange; i++)
			{
				Situation situationToRandonlyChange = GetRandomSituation();
				RobotAction action = GetRandomAction();
				strategy[situationToRandonlyChange] = action;
			}
		}

		public static void RandomlyChange(int minPercentageOfSituation, int maxPercentageOfSituation, IDictionary<Situation, RobotAction> strategy)
		{
			int itemsToChange = (int)(strategy.Count() * random.Next(minPercentageOfSituation, maxPercentageOfSituation+1) / 100.0);
			for (int i = 0; i < itemsToChange; i++)
			{
				Situation situationToRandonlyChange = GetRandomSituation();
				RobotAction action = GetRandomAction();
				strategy[situationToRandonlyChange] = action;
			}
		}


		public static RobotAction GetRandomAction()
		{
			return (RobotAction)random.Next(0, _actionCount);
		}

		public static Situation GetRandomSituation()
		{
			return new Situation((Situation.BoardContents)random.Next(0, _contentCount),
				(Situation.BoardContents)random.Next(0, _contentCount),
				(Situation.BoardContents)random.Next(0, _contentCount),
				(Situation.BoardContents)random.Next(0, _contentCount),
				(Situation.BoardContents)random.Next(0, _contentCount));
		}


	}
}