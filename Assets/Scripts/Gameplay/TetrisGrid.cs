using Tetrified.Scripts.Utility;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class TetrisGrid : Singleton<TetrisGrid>
    {
        private TetrisGrid()
        {

        }

        public int _width;
        public int _height;

        private int[,] _grid;

        public void AddPiece(Tetromino tetro)
        {

        }

        public int[,] GetGrid()
        {
            return _grid;
        }
    }
}