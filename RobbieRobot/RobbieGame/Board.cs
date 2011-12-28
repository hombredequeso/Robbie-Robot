using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MRC.RobbieRobot.RobbieGame
{
	public class Board
	{
		public int Width
		{
			get { return _playArea.GetLength(0); }
		}

		public int Height
		{
			get { return _playArea.GetLength(1); }
		}

		public ICollection<IBoardElement> Contents(Point p)
		{
			return new Collection<IBoardElement>(_playArea[p.X, p.Y].ToList());
		}

		public Point GetCurrentPosition(IBoardElement element)
		{
			return _currentPositions[element];
		}

		private readonly ICollection<IBoardElement>[,] _playArea;
		private readonly IDictionary<IBoardElement, Point> _currentPositions;

		public Board(int width, int height)
		{
			_currentPositions = new Dictionary<IBoardElement, Point>();
			_playArea = new Collection<IBoardElement>[width,height];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					_playArea[x, y] = new Collection<IBoardElement>();
		}

		public bool PointInsidePlayArea(Point p)
		{
			return (p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height);
		}

		public void AddElement(IBoardElement boardElement, Point position)
		{
			if (!PointInsidePlayArea(position))
				throw new ArgumentException("Position is not on board", "position");
			_playArea[position.X, position.Y].Add(boardElement);
			_currentPositions[boardElement] = position;
		}

		public void Move(IBoardElement boardElement, Direction direction)
		{
			var pos = _currentPositions[boardElement];
			var newPosition = GetNewPosition[direction](pos);
			if (!PointInsidePlayArea(newPosition))
				throw new InvalidMoveException();
			RemoveElement(boardElement);
			AddElement(boardElement, newPosition);
		}

		public enum Direction
		{
			North = 0,
			South,
			East,
			West
		}

		private readonly IDictionary<Direction, Func<Point, Point>> GetNewPosition =
			new Dictionary<Direction, Func<Point, Point>>()
				{
					{Direction.North, p => new Point(p.X - 1, p.Y)},
					{Direction.South, p => new Point(p.X + 1, p.Y)},
					{Direction.East, p => new Point(p.X, p.Y + 1)},
					{Direction.West, p => new Point(p.X, p.Y - 1)},
				};

		public void RemoveElement(IBoardElement boardElement)
		{
			if (boardElement == null) throw new ArgumentNullException("boardElement");
			if (!_currentPositions.ContainsKey(boardElement))
			{
				throw new ArgumentException("Element not on board", "boardElement");
			}
			var pos = _currentPositions[boardElement];
			_playArea[pos.X, pos.Y].Remove(boardElement);
			_currentPositions.Remove(boardElement);
		}
	}

	public class InvalidMoveException : Exception
	{
	}

	public struct Point : IEquatable<Point>
	{
		public Point(int x, int y) : this()
		{
			X = x;
			Y = y;
		}

		public int X { get; private set; }
		public int Y { get; private set; }

		public bool Equals(Point other)
		{
			return other.X == X && other.Y == Y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (obj.GetType() != typeof (Point)) return false;
			return Equals((Point) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X*397) ^ Y;
			}
		}

		public static bool operator ==(Point left, Point right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Point left, Point right)
		{
			return !left.Equals(right);
		}

		public static Point operator +(Point left, Point right)
		{
			return new Point(left.X + right.X, left.Y + right.Y);
		}
	}

	public interface IBoardElement
	{
	}
}