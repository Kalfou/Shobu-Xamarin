using System;

namespace Shobu.Persistence
{
    public class ShobuState
    {
        #region Fields

        private readonly int[,,] _board;

        #endregion

        #region Properties

        public int this[int b, int x, int y]
        {
            get => _board[b, x, y];
            set
            {
                if (b < 0 || b >= 4)
                {
                    throw new ArgumentOutOfRangeException("b", "The board index is out of range.");
                }

                if (x < 0 || x >= 4)
                {
                    throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
                }

                if (y < 0 || y >= 4)
                {
                    throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");
                }
                _board[b, x, y] = value;
            }
        }

        public int Turn { get; set; }

        #endregion

        #region Constructors

        public ShobuState()
        {
            _board = new int[4, 4, 4];
            Turn = 1;
        }

        #endregion

    }
}
