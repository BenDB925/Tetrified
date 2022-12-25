using Tetrified.Scripts.TetrominoLoading;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class Tetromino
    {
        // The current Tetris piece shape, including rotation
        private int[,] _shape;

        private int[,] _originalShapeOrientation;

        // The position of the Tetris piece on the grid
        private Vector2Int _posInGrid;

        // The rotation of the Tetris piece - numbered 0-4, 0 being initial, 1 being 90 degrees clockwise etc.
        private int _rotation;

        private Color _color;

        private TetrominoData.TetrominoName _name;

        public delegate void PlacementEvent();
        public static event PlacementEvent CantPlaceTetromino;

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
                do
                {
                    topY++;
                }
                while (IsValidMovement(midPointX, topY, randomShape._shape) == false);
            }




            _shape = randomShape._shape;
            _originalShapeOrientation = _shape;
            _color = randomShape._color;



            _posInGrid = new Vector2Int(midPointX, topY);
            Debug.Log("new " + _name + "piece placed at: " + _posInGrid);
            _rotation = 0;
        }

        // Rotates the Tetris piece clockwise
        public void Rotate()
        {
            // Calculate the new rotation
            int newRotation = (_rotation + 1) % 4;

            // Check if the new rotation is valid
            if (IsValidRotation(newRotation))
            {
                // Set the new rotation
                _rotation = newRotation;
            }
        }

        // Moves the Tetris piece to the left
        public void MoveLeft()
        {
            // Check if the Tetris piece can be moved to the left
            if (IsValidMovement(_posInGrid.x - 1, _posInGrid.y, _shape))
            {
                // Move the Tetris piece to the left
                _posInGrid.x--;
            }
        }

        // Moves the Tetris piece to the right
        public void MoveRight()
        {
            // Check if the Tetris piece can be moved to the right
            if (IsValidMovement(_posInGrid.x + 1, _posInGrid.y, _shape))
            {
                // Move the Tetris piece to the right
                _posInGrid.x++;
            }
        }

        // Moves the Tetris piece down
        public void MoveDown()
        {
            // Check if the Tetris piece can be moved down
            if (IsValidMovement(_posInGrid.x, _posInGrid.y - 1, _shape))
            {
                // Move the Tetris piece down
                _posInGrid.y--;

                Debug.Log(_name + " moving downwards, new y pos: " + _posInGrid.y);
            }
            else
            {
                Debug.Log(_name + " hit bottom, at y pos: " + _posInGrid.y);

                // The Tetris piece can't be moved down, so it has landed
                _gridData.AddPiece(this);
                NewPiece();
            }
        }

        // Checks if the Tetris piece can be moved to the specified position
        private bool IsValidMovement(int newX, int newY, int[,] shape)
        {
            bool isAtTheBottom = newY < 0;
            bool isOutOfXBounds = newX < 0 || newX + shape.GetLength(0) >= _gridData._width;

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
            // Get the dimensions of the array
            int xSize = shape.GetLength(0);
            int ySize = shape.GetLength(1);

            // Create a new array with the same dimensions
            int[,] rotated = new int[ySize, xSize];

            // Rotate the values in the array
            for (int t = 0; t < rotation; t++)
            {
                for (int i = 0; i < xSize; i++)
                {
                    for (int j = 0; j < ySize; j++)
                    {
                        rotated[j, xSize - i - 1] = shape[i, j];
                    }
                }
                shape = rotated;
            }

            return rotated;
        }

        // Checks if the Tetris piece can be rotated to the specified rotation
        private bool IsValidRotation(int newRotation)
        {
            // Rotate the Tetris piece to the new rotation
            int[,] rotatedShape = RotateShape(_originalShapeOrientation, newRotation);

            // Check if the rotated Tetris piece is out of bounds or collides with any other blocks on the grid
            for (int x = 0; x < rotatedShape.GetLength(0); x++)
            {
                for (int y = 0; y < rotatedShape.GetLength(1); y++)
                {
                    bool isPartOfTheShape = rotatedShape[x, y] != 0;
                    bool isOutOfXBounds = _posInGrid.x + x < 0 || _posInGrid.x + x >= _gridData._width;
                    bool isOutOfYBounds = _posInGrid.y + y < 0 || _posInGrid.y + y >= _gridData.GridHeightWithBufferRows;
                    bool isTileAlreadyOccupied = _gridData.GetGrid()[_posInGrid.x + x, _posInGrid.y + y]._shape != null;

                    if (isPartOfTheShape && (isOutOfXBounds || isOutOfYBounds || isTileAlreadyOccupied))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Gets the Tetris piece position
        public Vector2Int GetPositionInGrid()
        {
            return _posInGrid;
        }

        public int[,] GetShape()
        {
            return _shape;
        }

        public Color GetColor()
        {
            return _color;
        }
        public TetrominoData.TetrominoName GetName()
        {
            return _name;
        }
    }

}
