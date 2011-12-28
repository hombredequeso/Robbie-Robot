using System;
using System.Collections.Generic;
using System.Linq;

namespace MRC.RobbieRobot.RobbieGame
{
	public static class StrategyGenerator
	{
		private static Random _random;
		private static int _maxAction;

		static StrategyGenerator()
		{
			_random = new Random();
			_maxAction = Enum.GetValues(typeof (RobotAction)).Cast<RobotAction>().Max(x => (int) x);
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
								RobotAction a = (RobotAction)_random.Next(0, _maxAction + 1);
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
			while (e.MoveNext())
			{
				RobotAction e1 = e.Current.Value;
				RobotAction e2 = s2[e.Current.Key];
				var tossOfTheCoin = _random.Next(0, 2);
				var chosenAction = tossOfTheCoin == 0 ? e1 : e2;
				newStrategy.Add(e.Current.Key, chosenAction);
			}
			return newStrategy;
		}
	}
}