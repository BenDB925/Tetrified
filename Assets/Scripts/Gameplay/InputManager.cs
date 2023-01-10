using System.Collections.Generic;
using Tetrified.Scripts.Gameplay;
using Tetrified.Scripts.Utility;
using UnityEngine;

namespace Tetromino.Scripts.Gameplay
{
    public class InputManager : Singleton<InputManager>
    {
        // Enum to hold all the potential actions that a user can take in the game
        public enum Action
        {
            MoveTetronimoLeft,
            MoveTetronimoRight,
            SelectTetrisBoardLeft,
            SelectTetrisBoardRight,
            RotateClockwise,
            RotateCounterClockwise
        }

        // Dictionary to map actions to specific controls
        private Dictionary<Action, KeyCode> _controls = new Dictionary<Action, KeyCode>();


        // Start is called before the first frame update
        void Start()
        {
            _controls = KeybindLoader.Instance.LoadKeybinds();
        }

        // Update is called once per frame
        void Update()
        {
            // Check for user input on each action
            foreach (var action in _controls.Keys)
            {
                if (Input.GetKeyDown(_controls[action]))
                {
                    GameManager.Instance.HandleInputActions(action);
                }
            }

            CheckForTouchInputs();
            CheckForMouseInputs();
        }

        private void CheckForTouchInputs()
        {
            if (Input.touchCount > 0)
            {
                Vector2 touchPos = Input.GetTouch(0).position;
                HandleInput(touchPos);
            }
        }

        private void CheckForMouseInputs()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 touchPos = Input.mousePosition;
                HandleInput(touchPos);
            }
        }

        private void HandleInput(Vector2 inputPoint)
        {
            bool leftSide = (inputPoint.x / Screen.width) < 0.5f;
            bool bottomSide = (inputPoint.y / Screen.height) < 0.5f;

            if (leftSide && bottomSide && bottomSide)
            {
                GameManager.Instance.HandleInputActions(Action.RotateCounterClockwise);
            }
            else if (leftSide == false && bottomSide)
            {
                GameManager.Instance.HandleInputActions(Action.RotateClockwise);
            }
            else if (leftSide && bottomSide == false)
            {
                GameManager.Instance.HandleInputActions(Action.SelectTetrisBoardLeft);
            }
            else if (leftSide == false && bottomSide == false)
            {
                GameManager.Instance.HandleInputActions(Action.SelectTetrisBoardRight);
            }
        }
    }
}