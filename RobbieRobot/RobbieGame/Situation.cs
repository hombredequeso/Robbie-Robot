using System;

namespace MRC.RobbieRobot.RobbieGame
{
	public class Situation : IEquatable<Situation>
	{
		public enum BoardContents
		{
			empty = 0,
			can,
			wall
		};


		public Situation(BoardContents currentSite,
		                 BoardContents north,
		                 BoardContents south,
		                 BoardContents east,
		                 BoardContents west)
		{
			CurrentSite = currentSite;
			North = north;
			South = south;
			East = east;
			West = west;
		}

		public BoardContents CurrentSite { get; private set; }
		public BoardContents North { get; private set; }
		public BoardContents South { get; private set; }
		public BoardContents East { get; private set; }
		public BoardContents West { get; private set; }

		public bool Equals(Situation other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.CurrentSite, CurrentSite) && Equals(other.North, North) && Equals(other.South, South) &&
			       Equals(other.East, East) && Equals(other.West, West);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Situation)) return false;
			return Equals((Situation) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = CurrentSite.GetHashCode();
				result = (result*397) ^ North.GetHashCode();
				result = (result*397) ^ South.GetHashCode();
				result = (result*397) ^ East.GetHashCode();
				result = (result*397) ^ West.GetHashCode();
				return result;
			}
		}

		public static bool operator ==(Situation left, Situation right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Situation left, Situation right)
		{
			return !Equals(left, right);
		}
	}
}