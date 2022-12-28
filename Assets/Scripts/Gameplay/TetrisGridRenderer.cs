using System.Collections.Generic;
using Tetrified.Scripts.TetrominoLoading;
using Tetrified.Scripts.Utility;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class TetrisGridRenderer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _tetronimoBlockPrefab;

        [SerializeField]
        private TetrisGridData _gridData;

        [SerializeField]
        private ScaleToFillTransform _boardBackground;

        [SerializeField]
        private ScaleToFillTransform _boardSelector;

        [SerializeField]
        private ScaleToFillTransform _boardBorder;

        [SerializeField]
        private TetrisBoardLogicManager _logicManager;

        private GameObject[,] _landedTetrominoBlocks;

        private List<GameObject> _fallingTetrominoBlocks;

        [SerializeField]
        private Vector2 _gameboardDimensions;

        private Vector2 _blockDimensions;

        private Tetromino _fallingPiece;

        private void Start()
        {
            _blockDimensions = _gameboardDimensions / new Vector2Int(_gridData._width, _gridData._height);
            _fallingPiece = _logicManager.FallingPiece;
            _fallingTetrominoBlocks = new List<GameObject>();
            _landedTetrominoBlocks = new GameObject[_gridData._width, _gridData.GridHeightWithBufferRows];

            GetComponent<RectTransform>().sizeDelta = _gameboardDimensions;

            Vector2 parentSizeWithSelectionBuffer = _gameboardDimensions;
            parentSizeWithSelectionBuffer *= 1.25f;
            transform.parent.GetComponent<RectTransform>().sizeDelta = parentSizeWithSelectionBuffer;

            Vector2 boardPos = transform.localPosition;
            boardPos.y = ((parentSizeWithSelectionBuffer.y - _gameboardDimensions.y) * 0.5f) - (parentSizeWithSelectionBuffer.y * 0.5f);
            transform.localPosition = new Vector3(0, boardPos.y, 0);

            _boardBackground.RescaleToFillTransform();
            _boardSelector.RescaleToFillTransform();
            _boardBorder.RescaleToFillTransform();
        }

        void Update()
        {
            UpdateGameBoardRendering();
        }

        public void UpdateGameBoardRendering()
        {
            UpdateFallingPiece();
            UpdateLandedPieces();
        }

        private void UpdateFallingPiece()
        {
            foreach (GameObject block in _fallingTetrominoBlocks)
            {
                TetrominoBlockFactory.Instance.ReturnBlockToPool(block);
            }
            _fallingTetrominoBlocks.Clear();

            if (_fallingPiece != null)
            {
                int[,] fallingPieceShape = _fallingPiece.GetCurrentShape();

                for (int i = 0; i < fallingPieceShape.GetLength(0); i++)
                {
                    for (int j = 0; j < fallingPieceShape.GetLength(1); j++)
                    {
                        if (fallingPieceShape[i, j] != 0)
                        {
                            Vector2Int coordinate = new Vector2Int(i + _fallingPiece.GetPositionInGrid().x, j + _fallingPiece.GetPositionInGrid().y);

                            PlaceBlockAtCoordinate(coordinate, _fallingPiece.GetColor(), true);
                        }
                    }
                }
            }
        }

        private void UpdateLandedPieces()
        {
            for (int i = 0; i < _gridData._width; i++)
            {
                for (int j = 0; j < _gridData.GridHeightWithBufferRows; j++)
                {
                    TetrominoData dataForCurrentTile = _gridData.GetGrid()[i, j];

                    //if there should be a block here, and there isn't, add it
                    if (dataForCurrentTile._shape != null)
                    {
                        if (_landedTetrominoBlocks[i, j] == null)
                        {
                            PlaceBlockAtCoordinate(new Vector2Int(i, j), dataForCurrentTile._color, false);
                        }
                    }
                    else if (_landedTetrominoBlocks[i, j] != null)
                    {
                        TetrominoBlockFactory.Instance.ReturnBlockToPool(_landedTetrominoBlocks[i, j]);
                        _landedTetrominoBlocks[i, j] = null;
                    }

                }
            }
        }

        private void PlaceBlockAtCoordinate(Vector2Int coord, Color color, bool isFallingBlock)
        {
            Vector3 relativePos = new Vector3(coord.x * _blockDimensions.x - (_gameboardDimensions.x * 0.5f),
                                              coord.y * _blockDimensions.y,
                                                 0);

            GameObject newBlock = TetrominoBlockFactory.Instance.GetOrInstantiateBlock(relativePos, _blockDimensions, color, transform);

            if (isFallingBlock)
            {
                _fallingTetrominoBlocks.Add(newBlock);
            }
            else
            {
                _landedTetrominoBlocks[coord.x, coord.y] = newBlock;
            }
        }
    }

}