using Tetronimo.Scripts.Utility;
using UnityEngine.UIElements;

public class LeaderboardEntryManager
{
    Label _nameLabel;
    Label _scoreLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        _nameLabel = visualElement.Q<Label>("NameLabel");
        _scoreLabel = visualElement.Q<Label>("ScoreLabel");
    }

    public void SetData(OnlineScoreManager.ScoreData data)
    {
        _nameLabel.text = data.Name;
        _scoreLabel.text = data.Score.ToString();
    }
}
