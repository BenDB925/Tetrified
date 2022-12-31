using System;
using System.Collections.Generic;
using Tetrified.Scripts.TetrominoLoading;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class Tetromino
    {
        /// <summary>
        /// The layout of the tetronimo, 1's = part of the shape, 0's = blank
        /// </summary>
        private int[,] _originalShapeLayout;

        /// <summary>
        /// maps the block in the original shape to the current grid position in the board, and it's previous position.
        /// </summary>
        private Dictionary<Vector2Int, Tuple<Vector2Int, Vector2Int>> _oldAndCurrentGridPositions = new Dictionary<Vector2Int, Tuple<Vector2Int, Vector2Int>>();

        /// <summary>
        /// The position of the Tetris piece on the grid
        /// </summary>
        private Vector2Int _posInGrid;

        /// <summary>
        /// The rotation of the Tetris piece - numbered 0-4, 0 being initial, 1 being 90 degrees clockwise etc.
        /// </summary>
        private int _rotation;

        private Color _color;

        public delegate void PlacementEvent();
        public event PlacementEvent CantPlaceTetromino;
        public event PlacementEvent TetronimoLanded;
        public event PlacementEvent TetronimoMoved;

        private TetrisGridData _gridData;

        public Tetromino(TetrisGridData gridData)
        {
            _gridData = gridData;
            NewPiece();
        }

        ///\ <summary>
        /// Initializes a new Tetris piece
        /// </summary>
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

            _oldAndCurrentGridPositions.Clear();
        }

        /// <summary>
        /// Rotates the Tetris piece clockwise
        /// </summary>
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

                UpdatePiecePositionsInDictionary();
                TetronimoMoved?.Invoke();
            }
        }

        /// <summary>
        /// Moves the Tetris piece to the left
        /// </summary>
        public void MoveLeft()
        {
            // Check if the Tetris piece can be moved to the left
            if (IsValidMovement(_posInGrid.x - 1, _posInGrid.y, GetCurrentShape()))
            {
                // Move the Tetris piece to the left
                _posInGrid.x--;
                UpdatePiecePositionsInDictionary();
                TetronimoMoved?.Invoke();
            }
        }

        /// <summary>
        /// Moves the Tetris piece to the right
        /// </summary>
        public void MoveRight()
        {
            // Check if the Tetris piece can be moved to the right
            if (IsValidMovement(_posInGrid.x + 1, _posInGrid.y, GetCurrentShape()))
            {
                // Move the Tetris piece to the right
                _posInGrid.x++;
                UpdatePiecePositionsInDictionary();
                TetronimoMoved?.Invoke();
            }
        }

        /// <summary>
        /// Moves the Tetris piece down
        /// </summary>
        public void MoveDown()
        {
            // Check if the Tetris piece can be moved down
            if (IsValidMovement(_posInGrid.x, _posInGrid.y - 1, GetCurrentShape()))
            {
                // Move the Tetris piece down
                _posInGrid.y--;
                UpdatePiecePositionsInDictionary();
                TetronimoMoved?.Invoke();
            }
            else
            {
                // The Tetris piece can't be moved down, so it has landed
                _gridData.AddPiece(this);
                TetronimoLanded?.Invoke();
            }
        }

        /// <summary>
        /// Checks if the Tetris piece can be moved to the specified position
        /// </summary>
        /// <param name="newX">in grid coordinates</param>
        /// <param name="newY">in grid coordinates</param>
        /// <param name="shape">the current, rotated shape to check against</param>
        /// <returns></returns>
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

        /// <summary>
        /// Rotates the specified shape to the specified rotation
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="rotation">the rotation as a number between 0-3 in 90 degree increments</param>
        /// <returns></returns>
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

        private static int[,] RotateClockwiseOnce(int[,] currentShape)
        {
            int[,] newArray = new int[currentShape.GetLength(1), currentShape.GetLength(0)];
            int newY, newX = 0;
            for (int oldY = currentShape.GetLength(1) - 1; oldY >= 0; oldY--)
            {
                newY = 0;
                for (int oldX = 0; oldX < currentShape.GetLength(0); oldX++)
                {
                    newArray[newX, newY] = currentShape[oldX, oldY];
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

        /// <summary>
        /// Updates _oldAndCurrentGridPositions to contain the updated, rotated grid coords of each block in the tetronimo
        /// </summary>
        private void UpdatePiecePositionsInDictionary()
        {
            for (int i = 0; i < _originalShapeLayout.GetLength(0); i++)
            {
                for (int j = 0; j < _originalShapeLayout.GetLength(1); j++)
                {
                    Vector2Int coordInShape = new Vector2Int(i, j);

                    Vector2Int currCoord = FindCoordInGridForBlock(coordInShape);
                    Vector2Int oldCoord = currCoord;

                    if (_oldAndCurrentGridPositions.ContainsKey(coordInShape))
                    {
                        oldCoord = _oldAndCurrentGridPositions[coordInShape].Item2;
                    }

                    Tuple<Vector2Int, Vector2Int> oldAndCurrentPositionsForCoord = new Tuple<Vector2Int, Vector2Int>(oldCoord, currCoord);
                    _oldAndCurrentGridPositions[coordInShape] = oldAndCurrentPositionsForCoord;
                }
            }
        }

        private Vector2Int FindCoordInGridForBlock(Vector2Int coordInOriginalShape)
        {
            // Calculate the current grid position of the block based on its position in the original, unrotated tetromino
            // and the current rotation of the tetromino
            int currentBlockX = coordInOriginalShape.x;
            int currentBlockY = coordInOriginalShape.y;
            switch (_rotation)
            {
                case 1:
                    currentBlockX = coordInOriginalShape.y;
                    currentBlockY = _originalShapeLayout.GetLength(0) - 1 - coordInOriginalShape.x;
                    break;
                case 2:
                    currentBlockX = _originalShapeLayout.GetLength(0) - 1 - coordInOriginalShape.x;
                    currentBlockY = _originalShapeLayout.GetLength(1) - 1 - coordInOriginalShape.y;
                    break;
                case 3:
                    currentBlockX = _originalShapeLayout.GetLength(1) - 1 - coordInOriginalShape.y;
                    currentBlockY = coordInOriginalShape.x;
                    break;
            }

            return new Vector2Int(_posInGrid.x + currentBlockX, _posInGrid.y + currentBlockY);
        }

        public Tuple<Vector2Int, Vector2Int> GetOldAndCurrentPositionsForBlock(Vector2Int coordInOriginalShape)
        {

            if (_oldAndCurrentGridPositions.ContainsKey(coordInOriginalShape) == false)
            {
                Vector2Int currCoordInGrid = FindCoordInGridForBlock(coordInOriginalShape);

                Tuple<Vector2Int, Vector2Int> coords
                                = new Tuple<Vector2Int, Vector2Int>(currCoordInGrid, currCoordInGrid);

                _oldAndCurrentGridPositions[coordInOriginalShape] = coords;
            }

            return _oldAndCurrentGridPositions[coordInOriginalShape];
        }

        /// <summary>
        /// Gets the Tetronimo position in grid coords
        /// </summary>
        /// <returns></returns>
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

        public int GetRotation()
        {
            return _rotation;
        }

        public int[,] GetOriginalShape()
        {
            return _originalShapeLayout;
        }
    }

}
