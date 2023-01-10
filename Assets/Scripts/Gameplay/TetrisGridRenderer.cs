using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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

        private SpriteRenderer _selectorSprite;

        [SerializeField]
        private ScaleToFillTransform _boardBorder;

        [SerializeField]
        private TetrisBoardLogicManager _logicManager;

        private GameObject[,] _landedTetrominoBlocks;

        private Dictionary<Vector2Int, TransformLerper> _fallingTetronimoBlocks = new Dictionary<Vector2Int, TransformLerper>();

        private Vector2 _gameboardDimensions;
        private Vector2 _blockDimensions;

        private Tetromino _fallingPiece;

        private float _timeForLerp = 1;

        private bool _selected;

        [SerializeField]
        private float _selectionFadeTime = 0.5f;

        private float _timeSinceSelected;
        private float _alphaAtStart;

        bool _initialised = false;

        private void Start()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            //needed for horizontal layoutgroup to update this board's size.
            yield return new WaitForEndOfFrame();
            _initialised = true;

            _fallingPiece = _logicManager.FallingPiece;
            _fallingPiece.TetronimoMoved += OnFallingTetronimoMoved;
            _fallingPiece.TetronimoLanded += OnFallingPieceLanded;

            _landedTetrominoBlocks = new GameObject[_gridData._width, _gridData.GridHeightWithBufferRows];
            _selectorSprite = _boardSelector.GetComponent<SpriteRenderer>();

            _boardBackground.RescaleToFillTransform();
            _boardSelector.RescaleToFillTransform();
            _boardBorder.RescaleToFillTransform();

        }

        public void OnFallingTetronimoMoved()
        {
            if (_initialised == false)
            {
                return;
            }
            UpdateGameBoardRendering();
        }

        public void OnFallingPieceLanded()
        {
            if (_initialised == false)
            {
                return;
            }
            foreach (KeyValuePair<Vector2Int, TransformLerper> block in _fallingTetronimoBlocks)
            {
                TetrominoBlockFactory.Instance.ReturnBlockToPool(block.Value.gameObject);
            }
            _fallingTetronimoBlocks.Clear();

            UpdateLandedPieces();
        }

        public void UpdateSize()
        {
            float width = Mathf.Min(transform.parent.GetComponent<RectTransform>().rect.width, 500);
            float height = (width / _gridData._width) * _gridData._height;

            _gameboardDimensions = new Vector2(width, height);
            _blockDimensions = _gameboardDimensions / new Vector2Int(_gridData._width, _gridData._height);

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

            UpdateBlockPositioning();
        }

        public void UpdateGameBoardRendering()
        {
            UpdateFallingPiece();
            UpdateLandedPieces();
        }

        private void Update()
        {
            UpdateSelected();
        }

        private void UpdateFallingPiece()
        {
            if (_fallingPiece != null)
            {
                int[,] fallingPieceShape = _fallingPiece.GetOriginalShape();

                for (int i = 0; i < fallingPieceShape.GetLength(0); i++)
                {
                    for (int j = 0; j < fallingPieceShape.GetLength(1); j++)
                    {
                        if (fallingPieceShape[i, j] != 0)
                        {
                            Tuple<Vector2Int, Vector2Int> coordsForBlock = _fallingPiece.GetOldAndCurrentPositionsForBlock(new Vector2Int(i, j));

                            Vector2Int oldCoord = coordsForBlock.Item1;
                            Vector2Int currCoord = coordsForBlock.Item2;

                            PlaceBlockAtCoordinate(oldCoord, currCoord, new Vector2Int(i, j), _fallingPiece.GetColor(), true);
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
                            //no need for lerping now that the block has landed
                            PlaceBlockAtCoordinate(new Vector2Int(i, j), new Vector2Int(i, j), new Vector2Int(i, j), dataForCurrentTile._color, false);
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

        private void PlaceBlockAtCoordinate(Vector2Int oldCoordInGrid, Vector2Int destinationCoordInGrid, Vector2Int coordInShape, Color color, bool isFallingBlock)
        {
            Vector3 oldPos = GridCoordToWorldPos(new Vector2Int(oldCoordInGrid.x, oldCoordInGrid.y));
            Vector3 destinationPos = GridCoordToWorldPos(new Vector2Int(destinationCoordInGrid.x, destinationCoordInGrid.y));
            GameObject newBlock;
            TransformLerper blockLerper;

            if (isFallingBlock)
            {

                if (_fallingTetronimoBlocks.ContainsKey(coordInShape) == false)
                {
                    _fallingTetronimoBlocks.Add(coordInShape, null);
                }

                blockLerper = _fallingTetronimoBlocks[coordInShape];
                if (blockLerper == null)
                {
                    newBlock = TetrominoBlockFactory.Instance.GetOrInstantiateBlock(oldPos, _blockDimensions, color, transform, true);
                    blockLerper = newBlock.GetComponent<TransformLerper>();
                    _fallingTetronimoBlocks[coordInShape] = blockLerper;
                }
                blockLerper.SetDestination(destinationPos, _timeForLerp);
            }
            else
            {
                newBlock = TetrominoBlockFactory.Instance.GetOrInstantiateBlock(destinationPos, _blockDimensions, color, transform, false);
                _landedTetrominoBlocks[destinationCoordInGrid.x, destinationCoordInGrid.y] = newBlock;
            }

        }

        public void SetBoardSelected(bool selected)
        {
            if (_selectorSprite == null)
            {
                _selectorSprite = _boardSelector.GetComponent<SpriteRenderer>();
            }

            _selected = selected;
            _alphaAtStart = _selectorSprite.color.a;
            _timeSinceSelected = 0;
        }

        //updates alpha value for selector sprite based on whether this board is currently selected
        private void UpdateSelected()
        {
            _timeSinceSelected += Time.deltaTime;

            float goalAlpha;
            if (_selected)
            {
                goalAlpha = 1;
            }
            else
            {
                goalAlpha = 0;
            }

            float currAlpha = Mathf.Lerp(_alphaAtStart, goalAlpha, Mathf.Min(_timeSinceSelected / _selectionFadeTime, 1));

            Color currColor = _selectorSprite.color;
            currColor.a = currAlpha;
            _selectorSprite.color = currColor;
        }

        private Vector3 GridCoordToWorldPos(Vector2Int gridCoord)
        {
            return new Vector3(gridCoord.x * _blockDimensions.x - (_gameboardDimensions.x * 0.5f),
                               gridCoord.y * _blockDimensions.y - (_gameboardDimensions.y * 0.5f),
                                0);
        }

        private void UpdateBlockPositioning()
        {
            if (_landedTetrominoBlocks == null)
            {
                return;
            }

            for (int i = 0; i < _landedTetrominoBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < _landedTetrominoBlocks.GetLength(1); j++)
                {
                    var block = _landedTetrominoBlocks[i, j];

                    if (block == null)
                    {
                        continue;
                    }

                    Vector3 relativePos = GridCoordToWorldPos(new Vector2Int(i, j));

                    block.transform.localPosition = relativePos;
                    block.GetComponent<RectTransform>().sizeDelta = _blockDimensions;
                    block.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = _blockDimensions;
                }
            }

            foreach(var block in _fallingTetronimoBlocks)
            {
                GameObject blockObj = block.Value.gameObject;
                blockObj.GetComponent<RectTransform>().sizeDelta = _blockDimensions;
                blockObj.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = _blockDimensions;
            }
        }

        private void OnDestroy()
        {
            _fallingPiece.TetronimoMoved -= OnFallingTetronimoMoved;
        }

        public void SetLerpSpeed(float lerpSpeed)
        {
            _timeForLerp = lerpSpeed;
        }
    }

}