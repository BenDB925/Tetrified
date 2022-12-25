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
            RotateCounterclockwise,
            Drop,
            Pause
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
                    HandleAction(action);
                }
            }
        }

        // Method to handle the input for a specific action
        void HandleAction(Action action)
        {
            // Add code here to handle the input for each specific action
            switch (action)
            {
                case Action.MoveTetronimoLeft:
                    // Move the Tetris piece to the left
                    GameManager.Instance.HandleInputActions(Action.MoveTetronimoLeft);
                    break;
                case Action.MoveTetronimoRight:
                    // Move the Tetris piece to the right
                    GameManager.Instance.HandleInputActions(Action.MoveTetronimoRight);
                    break;
                case Action.SelectTetrisBoardLeft:
                    //Select the Tetris board to the left of the current
                    break;
                case Action.SelectTetrisBoardRight:
                    //Select the Tetris board to the right of the current
                    break;
                case Action.RotateClockwise:
                    // Rotate the Tetris piece clockwise
                    break;
                case Action.RotateCounterclockwise:
                    // Rotate the Tetris piece counterclockwise
                    break;
                case Action.Drop:
                    // Drop the Tetris piece
                    break;
                case Action.Pause:
                    // Pause the game
                    break;
            }
        }
    }
}