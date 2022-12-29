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
                    GameManager.Instance.HandleInputActions(action);
                }
            }
        }
    }
}