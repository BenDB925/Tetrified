
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Tetrified.Scripts.Utility
{
    public class MainMenuUIManager : Singleton<MainMenuUIManager>
    {
        [SerializeField]
        private UIDocument _rootDocument;

        private const string PlayGameButtonName = "PlayButton";
        private const string ControlsButtonName = "ControlsButton";

        private const string GameSceneName = "Game";
        private const string ControlsSceneName = "Controls";

        private void Start()
        {
            VisualElement rootElement = _rootDocument.rootVisualElement;
            Button playGameButton = rootElement.Query<Button>(PlayGameButtonName);
            playGameButton.clicked += OnPlayGamePressed;
            Button controlsButton = rootElement.Query<Button>(ControlsButtonName);
            controlsButton.clicked += OnViewControlsPressed;
        }

        private void OnPlayGamePressed()
        {
            SceneManager.LoadScene(GameSceneName);
        }
        private void OnViewControlsPressed()
        {
            SceneManager.LoadScene(ControlsSceneName);
        }
    }
}