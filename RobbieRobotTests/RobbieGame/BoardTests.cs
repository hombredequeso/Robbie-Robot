using System;
using System.Diagnostics;
using System.Linq;
using MRC.RobbieRobot.RobbieGame;
using NUnit.Framework;

namespace MRC.RobbieRobotTests.RobbieGame
{
	[TestFixture]
	public class BoardTests
	{
		[Test]
		public void NewBoard_Is_Correctly_Initialized()
		{
			const int width = 70;
			const int height = 80;


			Board board = new Board(width, height);

			Assert.AreEqual(width, board.Width);
			Assert.AreEqual(height, board.Height);
			for(int x=0; x<width; x++)
				for (int y=0; y<height; y++)
					Assert.IsTrue(board.Contents(new Point(x,y)).Count() == 0);
		}

		[Test]
		public void BoardLitterer_Can_Randomly_Place_Rubbish_On_Board()
		{
			var board = new Board(100, 100);
			var litterer = new BoardLitterer(0.5);
			litterer.Litter(board);

			int rubbishTotal = 0;
			for (int x = 0; x < board.Width; x++)
				for (int y = 0; y < board.Height; y++)
				{
					Assert.IsTrue(board.Contents(new Point(x, y)).Count() <= 1);
					if (board.Contents(new Point(x,y)).Count() == 1)
					{
						++rubbishTotal;
					}
				}
			Debug.Print("Rubbish count: {0}", rubbishTotal);
			Assert.IsTrue(rubbishTotal > 4000 && rubbishTotal < 6000,
				"Might fail on occasion, but is PROBABLY still working. Rerun test to check, if it failes.");
		}

		[Test]
		public void BoardElement_Can_Be_Added_To_Board()
		{
			var testElement = new TestBoardElement();
			var board = new Board(10, 20);
			Point position = new Point(5, 12);

			board.AddElement(testElement, position);

			Assert.IsTrue(board.Contents(position).Contains(testElement));
		}

		[Test]
		public void BoardElement_Cannot_Be_Added_To_Invalid_Position_On_Board()
		{
			// Arrange
			var testElement = new TestBoardElement();
			var board = new Board(10, 20);
			Point position = new Point(15, 12);
			Assert.Throws<ArgumentException>(() => board.AddElement(testElement, position));
		}

		[Test]
		public void BoardElement_Can_Be_Removed()
		{
			var testElement = new TestBoardElement();
			var board = new Board(10, 20);
			Point position = new Point(5, 12);
			board.AddElement(testElement, position);

			board.RemoveElement(testElement);

			Assert.IsFalse(board.Contents(position).Contains(testElement));
		}

		[Test]
		public void Element_Not_On_Board_Cannot_Be_Removed()
		{
			var elementNotOnBoard = new TestBoardElement();
			var board = new Board(20, 30);
			Assert.Throws<ArgumentException>(() => board.RemoveElement(elementNotOnBoard));
		}

		[Test]
		public void Element_Can_Be_Moved()
		{
			var testElement = new TestBoardElement();
			var board = new Board(10, 20);
			Point position = new Point(5, 12);
			board.AddElement(testElement, position);

			board.Move(testElement, Board.Direction.North);

			var expectedNewPosition = new Point(position.X - 1, position.Y);
			Assert.IsFalse(board.Contents(position).Contains(testElement));
			Assert.IsTrue(board.Contents(expectedNewPosition).Contains(testElement));
		}

		public class TestBoardElement: IBoardElement
		{}
	}
}