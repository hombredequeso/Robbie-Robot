using System.Collections.Generic;

namespace MRC.RobbieRobot.RobbieGame
{
	public class Scorer
	{
		public Scorer()
		{
			_scores = new Dictionary<Robot.ActionResult, int>()
			          	{
			          		{Robot.ActionResult.PickedUpCan, 10},
			          		{Robot.ActionResult.TriedToPickupCanWhereThereWasNoCan, -1},
			          		{Robot.ActionResult.HitWall, -5},
			          	};
		}

		public int Score { get; private set; }
		public IDictionary<Robot.ActionResult, int> _scores;

		public void Add(Robot.ActionResult actionResult)
		{
			if (_scores.ContainsKey(actionResult))
				Score += _scores[actionResult];
		}
	}
}