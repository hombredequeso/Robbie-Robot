using System;
using System.Collections.Generic;
using System.Linq;

namespace MRC.RobbieRobot.RobbieGame
{
	public class Robot: IBoardElement
	{
		private static Random random;

		static Robot()
		{
			random = new Random();
		}

		public IDictionary<Situation, RobotAction> _moveStrategy { get; private set; }

		public Robot(IDictionary<Situation, RobotAction> moveStrategy)
			:this(moveStrategy, null)
		{
		}

		public Robot(IDictionary<Situation, RobotAction> moveStrategy, Scorer scorer)
		{
			_moveStrategy = moveStrategy;
			_scorer = scorer;
		}

		public Scorer _scorer { get; private set; }


		public void Act(Board currentBoard)
		{
			Situation situation = LookAtCurrentSituation(currentBoard);
			RobotAction whatToDo = _moveStrategy[situation];
			var result = TakeAction(whatToDo, currentBoard);
			if (_scorer != null) _scorer.Add(result);
		}

		private ActionResult TakeAction(RobotAction whatToDo, Board board)
		{
			try
			{
				switch (whatToDo)
				{
					case RobotAction.StayPut:
						return ActionResult.DidNothing;
						break;
					case RobotAction.MoveEast:
						{
							board.Move(this, Board.Direction.East);
							return ActionResult.SuccessfulMove;
							break;
						}
					case RobotAction.MoveNorth:
						{
							board.Move(this, Board.Direction.North);
							return ActionResult.SuccessfulMove;
							break;
						}
					case RobotAction.MoveSouth:
						{
							board.Move(this, Board.Direction.South);
							return ActionResult.SuccessfulMove;
							break;
						}
					case RobotAction.MoveWest:
						{
							board.Move(this, Board.Direction.West);
							return ActionResult.SuccessfulMove;
							break;
						}
					case RobotAction.PickUpCan:
						{
							var pos = board.GetCurrentPosition(this);
							var can = board.Contents(pos).OfType<Rubbish>().FirstOrDefault();
							if (can == null) return ActionResult.TriedToPickupCanWhereThereWasNoCan;
							Pickup(board, can);
							return ActionResult.PickedUpCan;
							break;
						}
					case RobotAction.MoveRandom:
						{
							var randomDirection = (Board.Direction) random.Next(0, 4);
							board.Move(this, randomDirection);
							return ActionResult.PickedUpCan;
							break;
						}
					default:
						throw new ArgumentException("Nothing specified for action", "whatToDo");
				}
			} catch (InvalidMoveException e)
			{
				return ActionResult.HitWall;
			}
		}

		public enum ActionResult
		{
			SuccessfulMove = 0,
			HitWall,
			PickedUpCan,
			TriedToPickupCanWhereThereWasNoCan,
			DidNothing
		};

		private void Pickup(Board board, IBoardElement can)
		{
			board.RemoveElement(can);
		}


		private Situation LookAtCurrentSituation(Board board)
		{
			var pos = board.GetCurrentPosition(this);
			var current = GetContents(pos, board);
			var north = GetContents(pos + new Point(-1, 0), board);
			var south = GetContents(pos + new Point(1, 0), board);
			var east = GetContents(pos + new Point(0, 1), board);
			var west = GetContents(pos + new Point(0, -1), board);
			return new Situation(current, north, south, east, west);
		}

		private Situation.BoardContents GetContents(Point pos, Board board)
		{
			if (!board.PointInsidePlayArea(pos)) return Situation.BoardContents.wall;
			if (board.Contents(pos).Any(x => x.GetType() == typeof(Rubbish))) return Situation.BoardContents.can;
			return Situation.BoardContents.empty;
		}
	}

	public enum RobotAction
	{
		MoveNorth = 0,
		MoveSouth,
		MoveEast,
		MoveWest,
		MoveRandom,
		StayPut,
		PickUpCan
	};

}