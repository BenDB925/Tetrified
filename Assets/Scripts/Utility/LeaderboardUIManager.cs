using System.Collections.Generic;
using Tetrified.Scripts.Utility;
using Tetronimo.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LeaderboardUIManager : Singleton<LeaderboardUIManager>
{
    [SerializeField]
    private VisualTreeAsset _scoreAsset;

    [SerializeField]
    private UIDocument _rootDocument;

    private const string MainMenuButtonName = "BackButton";
    private const string MenuSceneName = "MainMenu";


    private void OnMenuPressed()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    private void Start()
    {
        VisualElement rootElement = _rootDocument.rootVisualElement;
        Button menuButton = rootElement.Query<Button>(MainMenuButtonName);
        menuButton.clicked += OnMenuPressed;
        OnlineScoreManager.Instance.RetrieveScores(OnScoresLoaded);
    }

    private void OnScoresLoaded(List<OnlineScoreManager.ScoreData> leaderboard)
    {
        PopulateLeaderboard(leaderboard);
    }

    private void PopulateLeaderboard(List<OnlineScoreManager.ScoreData> scores)
    {
        ListView leaderboard = _rootDocument.rootVisualElement.Query<ListView>("LeaderboardView");

        leaderboard.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = _scoreAsset.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new LeaderboardEntryManager();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        leaderboard.bindItem = (item, index) =>
        {
            (item.userData as LeaderboardEntryManager).SetData(scores[index]);
        };

        leaderboard.itemsSource = scores;
    }
}
