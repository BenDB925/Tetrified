using Tetrified.Scripts.TetrominoLoading;
using Tetrified.Scripts.Utility;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class TetrisGridData : Singleton<TetrisGridData>
    {
        protected override void Awake()
        {
            base.Awake();
            _grid = new TetrominoData[_width, GridHeightWithBufferRows];
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < GridHeightWithBufferRows; j++)
                {
                    _grid[i, j] = new TetrominoData()
                    {
                        _shape = null
                    };
                }
            }
        }

        public int _width;
        public int _height;

        private const int ExtraBufferRows = 10;

        private TetrominoData[,] _grid;

        //allows tetronimos to spawn half way off screen to better communicate game over
        public int GridHeightWithBufferRows
        {
            get
            {
                return _height + ExtraBufferRows;
            }
        }


        // Adds the blocks of the specified Tetris piece to the grid
        public void AddPiece(Tetromino piece)
        {
            int[,] shape = piece.GetShape();
            int xPos = piece.GetPositionInGrid().x;
            int yPos = piece.GetPositionInGrid().y;

            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] != 0)
                    {
                        _grid[xPos + i, yPos + j]._color = piece.GetColor();
                        _grid[xPos + i, yPos + j]._shape = piece.GetShape();
                        _grid[xPos + i, yPos + j]._name = piece.GetName();
                    }
                }
            }
        }

        // Removes any completed rows from the grid
        public void RemoveCompletedRows()
        {
            for (int r = 0; r < GridHeightWithBufferRows; r++)
            {
                // Check if the current row is completed
                bool completed = true;
                for (int c = 0; c < _width; c++)
                {
                    if (_grid[r, c]._shape == null)
                    {
                        completed = false;
                        break;
                    }
                }

                // If the current row is completed, remove it and move the rows above it down
                if (completed)
                {
                    for (int r2 = r; r2 > 0; r2--)
                    {
                        for (int c = 0; c < _width; c++)
                        {
                            _grid[r2, c] = _grid[r2 - 1, c];
                        }
                    }
                    r--;
                }
            }
        }

        public TetrominoData[,] GetGrid()
        {
            return _grid;
        }
    }
}