using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Action = Tetromino.Scripts.Gameplay.InputManager.Action;

namespace Tetrified.Scripts.Utility
{
    public class KeybindLoader : Singleton<KeybindLoader>
    {
        // Path to the JSON file where the keybinds will be saved
        [SerializeField]
        private string _keybindsFilePath;

        // Dictionary to map actions to specific controls
        private Dictionary<Action, KeyCode> _controls = new Dictionary<Action, KeyCode>();

        // Method to load the keybinds from the JSON file
        public Dictionary<Action, KeyCode> LoadKeybinds()
        {
            // Set default keybinds
            _controls[Action.MoveTetronimoLeft] = KeyCode.LeftArrow;
            _controls[Action.MoveTetronimoRight] = KeyCode.RightArrow;
            _controls[Action.SelectTetrisBoardLeft] = KeyCode.A;
            _controls[Action.SelectTetrisBoardRight] = KeyCode.D;
            _controls[Action.RotateClockwise] = KeyCode.UpArrow;
            _controls[Action.RotateCounterclockwise] = KeyCode.DownArrow;
            _controls[Action.Drop] = KeyCode.Space;
            _controls[Action.Pause] = KeyCode.Escape;

            // Check if the JSON file exists
            if (File.Exists(_keybindsFilePath))
            {
                // Read the contents of the file
                string json = File.ReadAllText(_keybindsFilePath);

                // Deserialize the JSON data into a dictionary
                Dictionary<string, KeyCode> keybinds = JsonConvert.DeserializeObject<Dictionary<string, KeyCode>>(json);

                // Loop through the dictionary and update the controls dictionary with the keybinds from the JSON file
                foreach (var keybind in keybinds)
                {
                    Tetromino.Scripts.Gameplay.InputManager.Action action;
                    if (Enum.TryParse(keybind.Key, out action))
                    {
                        _controls[action] = keybind.Value;
                    }
                }
            }

            return _controls;
        }
    }
}