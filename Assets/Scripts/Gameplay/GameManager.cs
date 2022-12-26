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

        private void Start()
        {
            _tetrisBoards = new List<TetrisBoardLogicManager>();
            CreateTetrisBoard();
        }

        public void HandleInputActions(InputManager.Action action)
        {
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

                default:
                    break;
            }
        }

        private void CreateTetrisBoard()
        {
            GameObject tetrisBoard = Instantiate(_tetrisBoardPrefab, Vector3.zero, Quaternion.identity);
            tetrisBoard.transform.SetParent(_tetrisBoardsParentTransform);
            tetrisBoard.transform.localScale = Vector3.one;
            _tetrisBoards.Add(tetrisBoard.GetComponent<TetrisBoardLogicManager>());
        }
    }
}