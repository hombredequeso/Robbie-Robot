using MRC.RobbieRobot.RobbieGame;
using NUnit.Framework;

namespace MRC.RobbieRobotTests.RobbieGame
{
	[TestFixture]
	public class ScoringTests
	{
		[Test]
		public void Can_Create_Scoring_Mechanism()
		{
			Scorer scorer = new Scorer();
			Assert.AreEqual(0, scorer.Score);
		}

		[Test]
		public void Scorer_Can_Give_Ten_Points_For_Picking_Up_A_Can()
		{
			Scorer scorer = new Scorer();
			scorer.Add(Robot.ActionResult.PickedUpCan);
			Assert.AreEqual(10, scorer.Score);
		}

		[Test]
		public void Scorer_Can_Give_Minus_One_Point_For_Trying_To_Pick_Up_A_Can_Where_There_Isnt_One()
		{
			Scorer scorer = new Scorer();
			scorer.Add(Robot.ActionResult.TriedToPickupCanWhereThereWasNoCan);
			Assert.AreEqual(-1, scorer.Score);
		}

		[Test]
		public void Scorer_Can_Give_Minus_Five_Points_For_Hitting_Wall()
		{
			Scorer scorer = new Scorer();
			scorer.Add(Robot.ActionResult.HitWall);
			Assert.AreEqual(-5, scorer.Score);
		}
	}
}