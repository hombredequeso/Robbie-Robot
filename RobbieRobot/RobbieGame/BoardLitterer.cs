using System;

namespace MRC.RobbieRobot.RobbieGame
{
	public class BoardLitterer
	{
		private readonly double _probabiltyOfSquareHavingRubbish;

		public BoardLitterer(double probabiltyOfSquareHavingRubbish)
		{
			_probabiltyOfSquareHavingRubbish = probabiltyOfSquareHavingRubbish;
		}

		public void Litter(Board board)
		{
			Random random = new Random();
			for (int x = 0; x < board.Width; x++)
				for (int y = 0; y < board.Height; y++)
				{
					if (random.NextDouble() < _probabiltyOfSquareHavingRubbish)
					{
						board.AddElement(new Rubbish(), new Point(x, y));
					}
				}
			
		}
	}
}