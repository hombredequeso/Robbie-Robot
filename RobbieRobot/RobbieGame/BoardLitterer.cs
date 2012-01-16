using System;
using System.Diagnostics;

namespace MRC.RobbieRobot.RobbieGame
{
	public class BoardLitterer
	{
		private readonly double _probabiltyOfSquareHavingRubbish;

		public BoardLitterer(double probabiltyOfSquareHavingRubbish)
		{
			_probabiltyOfSquareHavingRubbish = probabiltyOfSquareHavingRubbish;
		}

		[ThreadStatic] private static Random Random;

		private Random random { get { return Random ?? (Random = new Random()); } }

		public void Litter(Board board)
		{
			int litterCount = 0;
			for (int x = 0; x < board.Width; x++)
			{
				for (int y = 0; y < board.Height; y++)
				{
					if (random.NextDouble() < _probabiltyOfSquareHavingRubbish)
					{
						board.AddElement(new Rubbish(), new Point(x, y));
						++litterCount;
					}
				}
			}
			//Debug.WriteLine("Litter Count: {0}", litterCount);
		}
	}
}