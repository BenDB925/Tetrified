using Tetrified.Scripts.TetrominoLoading;
using Tetrified.Scripts.Utility;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class TetrisGridData : MonoBehaviour
    {
        private void Awake()
        {
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
            int[,] shape = piece.GetCurrentShape();
            int xPos = piece.GetPositionInGrid().x;
            int yPos = piece.GetPositionInGrid().y;

            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] != 0)
                    {
                        _grid[xPos + i, yPos + j]._color = piece.GetColor();
                        _grid[xPos + i, yPos + j]._shape = piece.GetCurrentShape();
                    }
                }
            }
        }

        // Removes any completed rows from the grid, returns if any rows were completed.
        public bool RemoveCompletedRows()
        {
            bool anyCompleted = false;

            for (int y = 0; y < GridHeightWithBufferRows; y++)
            {
                // Check if the current row is completed
                bool completed = true;
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[x, y]._shape == null)
                    {
                        completed = false;
                        break;
                    }
                }

                // If the current row is completed, remove it and move the rows above it down
                if (completed)
                {
                    for (int y2 = y; y2 < GridHeightWithBufferRows - 1; y2++)
                    {
                        for (int x = 0; x < _width; x++)
                        {
                            _grid[x, y2] = _grid[x, y2 + 1];
                        }
                    }
                    anyCompleted = true;
                }
            }

            //multiple rows can be completed at once.
            if (anyCompleted)
            {
                RemoveCompletedRows();
            }

            return anyCompleted;
        }

        public TetrominoData[,] GetGrid()
        {
            return _grid;
        }
    }
}