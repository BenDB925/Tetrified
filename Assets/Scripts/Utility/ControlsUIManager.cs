using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Tetrified.Scripts.Utility
{
    public class ControlsUIManager : Singleton<ControlsUIManager>
    {
        [SerializeField]
        private UIDocument _rootDocument;

        private const string MainMenuButtonName = "BackButton";

        private const string MenuSceneName = "MainMenu";

        private void Start()
        {
            VisualElement rootElement = _rootDocument.rootVisualElement;
            Button menuButton = rootElement.Query<Button>(MainMenuButtonName);
            menuButton.clicked += OnMenuPressed;
        }

        private void OnMenuPressed()
        {
            SceneManager.LoadScene(MenuSceneName);
        }
    }
}