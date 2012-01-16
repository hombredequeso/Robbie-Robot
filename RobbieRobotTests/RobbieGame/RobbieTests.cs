using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MRC.RobbieRobot.RobbieGame;
using NUnit.Framework;

namespace MRC.RobbieRobotTests.RobbieGame
{
	[TestFixture]
	public class RobbieTests
	{
		[Test]
		public void Strategy_Is_Created_With_Correct_Random_State()
		{
			var strategy = StrategyGenerator.Random();
			var possibilities = Math.Pow(3, 5); // 3 = BoardContent possibilites, 5 = parts to Situation.
			Assert.AreEqual(243, strategy.Count());

			int[] possibilityCounts = new int[7]; // 7 possibilities.
			foreach (var element in strategy)
			{
				++possibilityCounts[(int) element.Value];
			}

			Debug.Print("Possiblitiy counts: {0}", String.Join(": ", possibilityCounts));
			Assert.IsTrue(possibilityCounts.Min() > 15,
			              "Will PROBABLY pass. May randomly fail. Try running test again.");
			Assert.IsTrue(possibilityCounts.Max() < 50,
			              "Will PROBABLY pass. May randomly fail. Try running test again.");
		}

		[Test]
		public void Robot_Can_Move()
		{
			IDictionary<Situation, RobotAction> alwaysMoveSouthStrategy = GetSimpleStrategy(RobotAction.MoveSouth);
			Robot robbie = new Robot(alwaysMoveSouthStrategy);
			Board board = new Board(10, 10);
			Point initialPos = new Point(0, 0);
			board.AddElement(robbie, initialPos);
			robbie.Act(board);

			Assert.IsFalse(board.Contents(initialPos).Contains(robbie));
			Assert.IsTrue(board.Contents(initialPos + new Point(1, 0)).Contains(robbie));
		}

		[Test]
		public void Robbie_Can_Pickup_Rubbish()
		{
			IDictionary<Situation, RobotAction> alwaysPickupStrategy = GetSimpleStrategy(RobotAction.PickUpCan);
			Robot robbie = new Robot(alwaysPickupStrategy);
			Board board = new Board(10, 10);
			Point initialPos = new Point(0, 0);
			board.AddElement(robbie, initialPos);
			board.AddElement(new Rubbish(), initialPos);
			Assert.IsTrue(board.Contents(initialPos).Contains(robbie));

			robbie.Act(board);

			Assert.IsTrue(board.Contents(initialPos).OfType<Rubbish>().Count() == 0);
			Assert.IsTrue(board.Contents(initialPos).Contains(robbie));
		}

		[Test]
		public void Robbie_Can_Hit_Wall()
		{
			IDictionary<Situation, RobotAction> moveNorthStrategy = GetSimpleStrategy(RobotAction.MoveNorth);
			Robot robbie = new Robot(moveNorthStrategy);
			Board board = new Board(10, 10);
			Point initialPos = new Point(0, 0);
			board.AddElement(robbie, initialPos);

			robbie.Act(board);

			Assert.IsTrue(board.Contents(initialPos).Contains(robbie));
		}

		[Test]
		public void Robbie_Can_Fail_To_Pickup_Rubbish()
		{
			IDictionary<Situation, RobotAction> pickupStrategy = GetSimpleStrategy(RobotAction.PickUpCan);
			Robot robbie = new Robot(pickupStrategy);
			Board board = new Board(10, 10);
			Point initialPos = new Point(0, 0);
			board.AddElement(robbie, initialPos);
			Assert.IsTrue(board.Contents(initialPos).Contains(robbie));

			robbie.Act(board);

			Assert.IsTrue(board.Contents(initialPos).Contains(robbie));
		}

		[Test]
		public void Robbie_Can_Do_Nothing()
		{
			IDictionary<Situation, RobotAction> stayPutStrategy = GetSimpleStrategy(RobotAction.StayPut);
			Robot robbie = new Robot(stayPutStrategy);
			Board board = new Board(10, 10);
			Point initialPos = new Point(0, 0);
			board.AddElement(robbie, initialPos);
			Assert.IsTrue(board.Contents(initialPos).Contains(robbie));

			robbie.Act(board);

			Assert.IsTrue(board.Contents(initialPos).Contains(robbie));
		}

		[Test]
		public void Robbie_State_Change_Hash_Code_Remains_Constant()
		{
			IDictionary<Situation, RobotAction> strategy = GetSimpleStrategy(RobotAction.PickUpCan);
			Scorer scorer = new Scorer();
			Robot robbie = new Robot(strategy, scorer);
			var startingHashCode = robbie.GetHashCode();

			robbie._scorer.Add(Robot.ActionResult.HitWall);

			var endingHashCode = robbie.GetHashCode();

			Assert.AreEqual(startingHashCode, endingHashCode);
		}


		private IDictionary<Situation, RobotAction> GetSimpleStrategy(RobotAction robotAction)
		{
			var strategy = new Dictionary<Situation, RobotAction>();
			var contentPossibilities = Enum.GetValues(typeof (Situation.BoardContents)).Cast<Situation.BoardContents>().ToArray();
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
								strategy[s] = robotAction;
							}
						}
					}
				}
			}
			return strategy;
		}
	}
}