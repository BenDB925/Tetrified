using Tetrified.Scripts.TetrominoLoading;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class Tetromino : MonoBehaviour
    {
        // The current Tetris piece shape
        private int[,] _shape;

        // The position of the Tetris piece on the grid
        private int _row;
        private int _col;

        // The rotation of the Tetris piece - numbered 0-4, 0 being initial, 1 being 90 degrees clockwise etc.
        private int _rotation;

        private void Start()
        {
            NewPiece();
        }

        // Initializes a new Tetris piece
        public void NewPiece()
        {
            // Randomly select a Tetris piece shape
            _shape = TetrominoShapeJsonLoader.Instance.GetRandomShapeFromJson();

            // Set the initial position and rotation of the Tetris piece
            _row = 0;
            _col = TetrisGrid.Instance._width / 2 - _shape.GetLength(1) / 2;
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
            if (IsValidMovement(_row, _col - 1))
            {
                // Move the Tetris piece to the left
                _col--;
            }
        }

        // Moves the Tetris piece to the right
        public void MoveRight()
        {
            // Check if the Tetris piece can be moved to the right
            if (IsValidMovement(_row, _col + 1))
            {
                // Move the Tetris piece to the right
                _col++;
            }
        }

        // Moves the Tetris piece down
        public void MoveDown()
        {
            // Check if the Tetris piece can be moved down
            if (IsValidMovement(_row + 1, _col))
            {
                // Move the Tetris piece down
                _row++;
            }
            else
            {
                // The Tetris piece can't be moved down, so it has landed
                TetrisGrid.Instance.AddPiece(this);
                NewPiece();
            }
        }

        // Checks if the Tetris piece can be moved to the specified position
        private bool IsValidMovement(int newRow, int newCol)
        {
            // Check if the Tetris piece is out of bounds
            if (newRow < 0 || newRow + _shape.GetLength(0) > TetrisGrid.Instance._height || newCol < 0 || newCol + _shape.GetLength(1) > TetrisGrid.Instance._width)
            {
                return false;
            }

            // Check if the Tetris piece collides with any other blocks on the grid
            for (int r = 0; r < _shape.GetLength(0); r++)
            {
                for (int c = 0; c < _shape.GetLength(1); c++)
                {
                    if (_shape[r, c] != 0 && TetrisGrid.Instance.GetGrid()[newRow + r, newCol + c] != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Rotates the specified shape to the specified rotation
        private int[,] RotateShape(int[,] shape, int rotation)
        {
            // Create a new array for the rotated shape
            int[,] rotatedShape = new int[shape.GetLength(1), shape.GetLength(0)];

            // Rotate the shape
            for (int r = 0; r < shape.GetLength(0); r++)
            {
                for (int c = 0; c < shape.GetLength(1); c++)
                {
                    rotatedShape[c, shape.GetLength(0) - 1 - r] = shape[r, c];
                }
            }

            return rotatedShape;
        }

        // Checks if the Tetris piece can be rotated to the specified rotation
        private bool IsValidRotation(int newRotation)
        {
            // Rotate the Tetris piece to the new rotation
            int[,] rotatedShape = RotateShape(_shape, newRotation);

            // Check if the rotated Tetris piece is out of bounds or collides with any other blocks on the grid
            for (int r = 0; r < rotatedShape.GetLength(0); r++)
            {
                for (int c = 0; c < rotatedShape.GetLength(1); c++)
                {
                    if (rotatedShape[r, c] != 0 && (_row + r < 0 || _row + r >= TetrisGrid.Instance._height || _col + c < 0 || _col + c >= TetrisGrid.Instance._width || TetrisGrid.Instance.GetGrid()[_row + r, _col + c] != 0))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public enum RotationDirection
        {
            Clockwise,
            Anticlockwise
        }

        public void RotateInDirection(RotationDirection direction)
        {
            // Create a new array to store the rotated blocks
            int[,] rotatedBlocks = new int[_shape.GetLength(1), _shape.GetLength(0)];

            // Loop through each block in the original array
            for (int i = 0; i < _shape.GetLength(0); i++)
            {
                for (int j = 0; j < _shape.GetLength(1); j++)
                {
                    // Rotate the block and add it to the new array
                    if (direction == RotationDirection.Clockwise)
                    {
                        rotatedBlocks[j, _shape.GetLength(0) - i - 1] = _shape[i, j];
                    }
                    else
                    {
                        rotatedBlocks[_shape.GetLength(1) - j - 1, i] = _shape[i, j];
                    }
                }
            }

            // Replace the original array with the rotated array
            _shape = rotatedBlocks;
        }

        // Gets the Tetris piece position
        public int GetRow()
        {
            return _row;
        }

        public int GetCol()
        {
            return _col;
        }
    }

}
