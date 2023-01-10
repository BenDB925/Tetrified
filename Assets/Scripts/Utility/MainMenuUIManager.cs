using LootLocker.Requests;
using Tetronimo.Scripts.Utility;
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
        private const string LeaderboardButtonName = "LeaderboardButton";

        private const string GameSceneName = "Game";
        private const string ControlsSceneName = "Controls";
        private const string LeaderboardSceneName = "Leaderboard";

        private void Start()
        {
            VisualElement rootElement = _rootDocument.rootVisualElement;
            Button playGameButton = rootElement.Query<Button>(PlayGameButtonName);
            playGameButton.clicked += OnPlayGamePressed;
            Button controlsButton = rootElement.Query<Button>(ControlsButtonName);
            controlsButton.clicked += OnViewControlsPressed;
            Button leaderboardButton = rootElement.Query<Button>(LeaderboardButtonName);
            leaderboardButton.clicked += OnLeaderboardPressed;
            OnlineScoreManager.Instance.InitSession();
        }

        private void OnPlayGamePressed()
        {
            SceneManager.LoadScene(GameSceneName);
        }
        private void OnViewControlsPressed()
        {
            SceneManager.LoadScene(ControlsSceneName);
        }
        private void OnLeaderboardPressed()
        {
            SceneManager.LoadScene(LeaderboardSceneName);
        }
    }
}