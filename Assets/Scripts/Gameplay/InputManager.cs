using System;
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

            CheckForMouseInputs();
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
            float xClickPercent = (inputPoint.x / Screen.width);
            float yClickPercent = (inputPoint.y / Screen.height);

            int xThird = Mathf.FloorToInt((int)(xClickPercent * 9) / 3);
            int yThird = Mathf.FloorToInt((int)(yClickPercent * 9) / 3);

            switch (xThird)
            {
                case 0:
                    switch (yThird)
                    {
                        //bottom left third
                        case 0:
                            GameManager.Instance.HandleInputActions(Action.MoveTetronimoLeft);
                            break;

                        //middle left third
                        case 1:
                            GameManager.Instance.HandleInputActions(Action.RotateCounterClockwise);
                            break;

                        //top left third
                        case 2:
                            GameManager.Instance.HandleInputActions(Action.SelectTetrisBoardLeft);
                            break;
                    }
                    break;
                case 2:
                    switch (yThird)
                    {
                        //bottom right third
                        case 0:
                            GameManager.Instance.HandleInputActions(Action.MoveTetronimoRight);
                            break;

                        //middle right third
                        case 1:
                            GameManager.Instance.HandleInputActions(Action.RotateClockwise);
                            break;

                        //top right third
                        case 2:
                            GameManager.Instance.HandleInputActions(Action.SelectTetrisBoardRight);
                            break;
                    }
                    break;
            }
        }
    }
}