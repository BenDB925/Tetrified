using Tetrified.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Tetrified.Scripts.Utility
{
    public class GameOverUIManager : Singleton<GameOverUIManager>
    {
        [SerializeField]
        private UIDocument _rootDocument;

        [SerializeField]
        private VisualTreeAsset _gameOverUI;

        private const string RetryButtonName = "RetryButton";
        private const string MainMenuButtonName = "MainMenuButton";
        private const string PointsValueText = "PointsValue";

        private const string GameSceneName = "Game";
        private const string ControlsSceneName = "Controls";

        private void Init(string scoreString)
        {
            VisualElement rootElement = _rootDocument.rootVisualElement;
            Button playGameButton = rootElement.Query<Button>(RetryButtonName);
            playGameButton.clicked += OnPlayGamePressed;
            Button menuButton = rootElement.Query<Button>(MainMenuButtonName);
            menuButton.clicked += OnMenuPressed;
            TextElement pointsText = rootElement.Query<TextElement>(PointsValueText);
            pointsText.text = scoreString;
        }

        public void EnableGameOverUI(int score)
        {
            _rootDocument.visualTreeAsset = _gameOverUI;
            string scoreString = UIManager.GetScoreString(score);
            Init(scoreString);
        }

        private void OnPlayGamePressed()
        {
            SceneManager.LoadScene(GameSceneName);
        }
        private void OnMenuPressed()
        {
            SceneManager.LoadScene(ControlsSceneName);
        }
    }
}