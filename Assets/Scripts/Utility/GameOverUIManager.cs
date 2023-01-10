using Tetrified.Scripts.Gameplay;
using Tetronimo.Scripts.Utility;
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
        private const string SubmitScoreButtonName = "SubmitScoreButton";
        private const string PointsValueText = "PointsValueGameOver";

        private const string UserNameField = "NameField";

        private const string GameSceneName = "Game";
        private const string ControlsSceneName = "Controls";
        private const string MenuSceneName = "MainMenu";

        private void Init(string scoreString)
        {
            VisualElement rootElement = _rootDocument.rootVisualElement;
            Button playGameButton = rootElement.Query<Button>(RetryButtonName);
            playGameButton.clicked += OnPlayGamePressed;
            Button submitScoreButton = rootElement.Query<Button>(SubmitScoreButtonName);
            submitScoreButton.clicked += OnSubmitPressed;
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
            SceneManager.LoadScene(MenuSceneName);
        }

        private void OnSubmitPressed()
        {
            VisualElement rootElement = _rootDocument.rootVisualElement;
            TextField userName = rootElement.Query<TextField>(UserNameField);
            SubmitScore(userName.text, PointsManager.Instance.GetPointsCount());
        }

        private void SubmitScore(string name, int score)
        {
            OnlineScoreManager.Instance.SubmitScore(name, score);
        }
    }
}