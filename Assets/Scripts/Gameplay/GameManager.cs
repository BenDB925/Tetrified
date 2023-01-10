using System.Collections;
using System.Collections.Generic;
using Tetrified.Scripts.Utility;
using Tetromino.Scripts.Gameplay;
using UnityEngine;

namespace Tetrified.Scripts.Gameplay
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private GameObject _tetrisBoardPrefab;

        private List<TetrisBoardLogicManager> _tetrisBoards;

        private int _currentActiveBoard;

        [SerializeField]
        private Transform _tetrisBoardsParentTransform;

        //time to spawn another board increases each time, this is the initial delay.
        [SerializeField]
        private float _baseTimeToSpawnExtraBoard = 15;

        private const float _boardSpawnTimeScalar = 2.0f;
        private float _currTimeToSpawnBoard;

        private float _timeSinceBoardSpawned;

        [SerializeField]
        private float _speedAccel = 0.1f;

        private float _currSpeed = 1;

        [SerializeField]
        private RectTransform _canvasRect;

        private bool _isGameOver;

        private void Start()
        {
            _isGameOver = false;
            _tetrisBoards = new List<TetrisBoardLogicManager>();
            CreateTetrisBoard();
            _currTimeToSpawnBoard = _baseTimeToSpawnExtraBoard;
            SelectTetrisBoard();

            TetrisBoardLogicManager.GameOverEvent += OnGameOver;
            PointsManager.Instance.SetEarningPoints(true);
        }

        private void OnGameOver()
        {
            _isGameOver = true;
            foreach (TetrisBoardLogicManager tetrisBoard in _tetrisBoards)
            {
                tetrisBoard.SetPaused(true);
            }

            PointsManager.Instance.SetEarningPoints(false);
            GameOverUIManager.Instance.EnableGameOverUI(PointsManager.Instance.GetPointsCount());
        }

        public void HandleInputActions(InputManager.Action action)
        {
            if (_isGameOver)
            {
                return;
            }

            switch (action)
            {
                case InputManager.Action.MoveTetronimoLeft:
                    if (_tetrisBoards?.Count > _currentActiveBoard)
                    {
                        _tetrisBoards[_currentActiveBoard].TakeInput(InputManager.Action.MoveTetronimoLeft);
                    }
                    break;
                case InputManager.Action.MoveTetronimoRight:
                    if (_tetrisBoards?.Count > _currentActiveBoard)
                    {
                        _tetrisBoards[_currentActiveBoard].TakeInput(InputManager.Action.MoveTetronimoRight);
                    }
                    break;
                case InputManager.Action.RotateClockwise:
                    if (_tetrisBoards?.Count > _currentActiveBoard)
                    {
                        _tetrisBoards[_currentActiveBoard].TakeInput(InputManager.Action.RotateClockwise);
                    }
                    break;

                case InputManager.Action.SelectTetrisBoardLeft:
                    _currentActiveBoard--;
                    if (_currentActiveBoard < 0)
                    {
                        _currentActiveBoard = _tetrisBoards.Count - 1;
                    }
                    SelectTetrisBoard();
                    break;

                case InputManager.Action.SelectTetrisBoardRight:
                    _currentActiveBoard++;
                    if (_currentActiveBoard > _tetrisBoards.Count - 1)
                    {
                        _currentActiveBoard = 0;
                    }
                    SelectTetrisBoard();
                    break;

                default:
                    break;
            }
        }

        private void CreateTetrisBoard()
        {
            GameObject tetrisBoard = Instantiate(_tetrisBoardPrefab, Vector3.zero, Quaternion.identity);
            tetrisBoard.transform.SetParent(_tetrisBoardsParentTransform);
            tetrisBoard.transform.localScale = Vector3.one;
            _tetrisBoards.Add(tetrisBoard.GetComponentInChildren<TetrisBoardLogicManager>());
            StartCoroutine(UpdateSizes());
        }

        /// <summary>
        /// updates the scale of the tetris boards based on canvas size and tetris board count
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateSizes()
        {
            yield return new WaitForEndOfFrame();

            foreach (TetrisBoardLogicManager board in _tetrisBoards)
            {
                board.UpdateSize();
            }

            float playAreaX = _canvasRect.sizeDelta.x * 0.6f;
            float playAreaY = _canvasRect.sizeDelta.y * 0.6f;
            float totalBoardWidth = 0;
            float boardHeight = _tetrisBoards[0].GetComponent<RectTransform>().sizeDelta.y;

            foreach (TetrisBoardLogicManager board in _tetrisBoards)
            {
                totalBoardWidth += board.GetComponent<RectTransform>().rect.width;
            }

            float xScale = Mathf.Min(playAreaX / totalBoardWidth, 0.8f);
            float yScale = Mathf.Min(playAreaY / boardHeight, 0.8f);

            float newScale = Mathf.Min(xScale, yScale);

            foreach (TetrisBoardLogicManager board in _tetrisBoards)
            {
                board.transform.parent.localScale = new Vector3(newScale, newScale, 1);
            }
        }

        private void SelectTetrisBoard()
        {
            for (int i = 0; i < _tetrisBoards.Count; i++)
            {
                TetrisBoardLogicManager board = _tetrisBoards[i];
                board.SetBoardSelected(_currentActiveBoard == i);
            }

        }

        private void Update()
        {
            if (_isGameOver)
            {
                return;
            }

            _timeSinceBoardSpawned += Time.deltaTime;

            if (_timeSinceBoardSpawned > _currTimeToSpawnBoard)
            {
                _currTimeToSpawnBoard *= _boardSpawnTimeScalar;
                _timeSinceBoardSpawned = 0;
                CreateTetrisBoard();
            }

            _currSpeed += _speedAccel * Time.deltaTime;

            foreach (TetrisBoardLogicManager board in _tetrisBoards)
            {
                board._tetrominoFallSpeed = _currSpeed;
            }
        }

        private void OnDestroy()
        {
            TetrisBoardLogicManager.GameOverEvent -= OnGameOver;
        }
    }
}