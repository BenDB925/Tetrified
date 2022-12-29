using Tetrified.Scripts.TetrominoLoading;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class Tetromino
    {
        // The current Tetris piece shape, including rotation
        private int[,] _originalShapeLayout;

        // The position of the Tetris piece on the grid
        private Vector2Int _posInGrid;

        // The rotation of the Tetris piece - numbered 0-4, 0 being initial, 1 being 90 degrees clockwise etc.
        private int _rotation;

        private Color _color;

        public delegate void PlacementEvent();
        public event PlacementEvent CantPlaceTetromino;
        public event PlacementEvent TetronimoLanded;

        private TetrisGridData _gridData;

        public Tetromino(TetrisGridData gridData)
        {
            _gridData = gridData;
            NewPiece();
        }

        // Initializes a new Tetris piece
        public void NewPiece()
        {
            // Randomly select a Tetris piece shape
            TetrominoData randomShape = TetrominoShapeJsonLoader.Instance.GetRandomShapeFromJson();

            // Set the initial position and rotation of the Tetris piece
            int boardWidth = _gridData._width;
            int midPointX = (boardWidth / 2) - (randomShape._shape.GetLength(0) / 2);
            int topY = _gridData._height - randomShape._shape.GetLength(1);

            bool isGameOver = IsValidMovement(midPointX, topY, randomShape._shape) == false;

            if (isGameOver)
            {
                CantPlaceTetromino?.Invoke();
            }

            _originalShapeLayout = randomShape._shape;
            _color = randomShape._color;

            _posInGrid = new Vector2Int(midPointX, topY);
            _rotation = 0;
        }

        // Rotates the Tetris piece clockwise
        public void Rotate()
        {
            // Calculate the new rotation
            int newRotation = (_rotation + 1) % 4;

            Vector2Int movementNeeded;
            bool canRotate = MovementNeededForRotation(newRotation, out movementNeeded);
            if (canRotate)
            {
                _posInGrid += movementNeeded;

                // Set the new rotation
                _rotation = newRotation;
            }
        }

        // Moves the Tetris piece to the left
        public void MoveLeft()
        {
            // Check if the Tetris piece can be moved to the left
            if (IsValidMovement(_posInGrid.x - 1, _posInGrid.y, GetCurrentShape()))
            {
                // Move the Tetris piece to the left
                _posInGrid.x--;
            }
        }

        // Moves the Tetris piece to the right
        public void MoveRight()
        {
            // Check if the Tetris piece can be moved to the right
            if (IsValidMovement(_posInGrid.x + 1, _posInGrid.y, GetCurrentShape()))
            {
                // Move the Tetris piece to the right
                _posInGrid.x++;
            }
        }

        // Moves the Tetris piece down
        public void MoveDown()
        {
            // Check if the Tetris piece can be moved down
            if (IsValidMovement(_posInGrid.x, _posInGrid.y - 1, GetCurrentShape()))
            {
                // Move the Tetris piece down
                _posInGrid.y--;
            }
            else
            {
                // The Tetris piece can't be moved down, so it has landed
                _gridData.AddPiece(this);
                TetronimoLanded?.Invoke();
            }
        }

        // Checks if the Tetris piece can be moved to the specified position
        private bool IsValidMovement(int newX, int newY, int[,] shape)
        {
            bool isAtTheBottom = newY < 0;
            bool isOutOfXBounds = newX < 0 || newX + shape.GetLength(0) > _gridData._width;

            // Check if the Tetris piece is out of bounds
            if (isAtTheBottom || isOutOfXBounds)
            {
                return false;
            }

            // Check if the Tetris piece collides with any other blocks on the grid
            for (int x = 0; x < shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.GetLength(1); y++)
                {
                    bool isPartOfTetromino = shape[x, y] != 0;
                    TetrominoData[,] grid = _gridData.GetGrid();
                    TetrominoData tile = grid[newX + x, newY + y];

                    if (isPartOfTetromino && tile._shape != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Rotates the specified shape to the specified rotation
        static int[,] RotateShape(int[,] shape, int rotation)
        {
            int[,] rotated = shape;

            // Rotate the values in the array
            for (int t = 0; t < rotation; t++)
            {
                rotated = RotateClockwiseOnce(shape);
                shape = rotated;
            }

            return rotated;
        }

        private static int[,] RotateClockwiseOnce(int[,] oldArray)
        {
            int[,] newArray = new int[oldArray.GetLength(1), oldArray.GetLength(0)];
            int newY, newX = 0;
            for (int oldY = oldArray.GetLength(1) - 1; oldY >= 0; oldY--)
            {
                newY = 0;
                for (int oldX = 0; oldX < oldArray.GetLength(0); oldX++)
                {
                    newArray[newX, newY] = oldArray[oldX, oldY];
                    newY++;
                }
                newX++;
            }
            return newArray;
        }


        /// <summary>
        /// Checks if the Tetris piece can be rotated to the specified rotation.
        /// </summary>
        /// <param name="newRotation"></param>
        /// <param name="adjustedPos">if near the edge, the push back needed to be able to rotate</param>
        /// <returns>if, after rotating and pushing away from the edge, if it's a valid movement (not colliding w/ anything)</returns>
        private bool MovementNeededForRotation(int newRotation, out Vector2Int adjustedPos)
        {
            adjustedPos = new Vector2Int();
            // Rotate the Tetris piece to the new rotation
            int[,] rotatedShape = RotateShape(_originalShapeLayout, newRotation);

            // Check if the rotated Tetris piece is out of bounds or collides with any other blocks on the grid
            for (int x = 0; x < rotatedShape.GetLength(0); x++)
            {
                for (int y = 0; y < rotatedShape.GetLength(1); y++)
                {
                    bool isPartOfTheShape = rotatedShape[x, y] != 0;

                    if (isPartOfTheShape)
                    {
                        bool isMinusX = _posInGrid.x + x < 0;
                        if (isMinusX)
                        {
                            adjustedPos.x = Mathf.Max(-(_posInGrid.x + x), adjustedPos.x);
                        }

                        bool isMaxX = _posInGrid.x + x >= _gridData._width;
                        if (isMaxX)
                        {
                            adjustedPos.x = Mathf.Min((_gridData._width - 1) - (_posInGrid.x + x), adjustedPos.x);
                        }

                        bool isMinusY = _posInGrid.x + x < 0;
                        if (isMinusY)
                        {
                            adjustedPos.y = Mathf.Max(-(_posInGrid.y + y), adjustedPos.y);
                        }

                        bool isMaxY = _posInGrid.y + y >= _gridData.GridHeightWithBufferRows;
                        if (isMaxY)
                        {
                            adjustedPos.y = Mathf.Min((_gridData.GridHeightWithBufferRows - 1) - (_posInGrid.y + y), adjustedPos.y);
                        }
                    }
                }
            }

            return IsValidMovement(_posInGrid.x + adjustedPos.x, _posInGrid.y + adjustedPos.y, rotatedShape);
        }

        // Gets the Tetris piece position
        public Vector2Int GetPositionInGrid()
        {
            return _posInGrid;
        }

        public int[,] GetCurrentShape()
        {
            return RotateShape(_originalShapeLayout, _rotation);
        }

        public Color GetColor()
        {
            return _color;
        }
    }

}
