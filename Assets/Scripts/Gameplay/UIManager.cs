using System.Collections.Generic;
using Tetrified.Scripts.Utility;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private UIDocument _rootDocument;

    private TextElement _pointsText;
    private const string PointsTextGameName = "PointsValue";
    private const string PointsTextGameOverName = "PointsValueGameOver";

    private void Start()
    {
        VisualElement rootElement = _rootDocument.rootVisualElement;
        _pointsText = rootElement.Query<TextElement>(PointsTextGameName);
    }

    public void SetScore(int score)
    {
        _pointsText.text = GetScoreString(score);
    }

    /// <summary>
    /// returns a formatted string with 
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static string GetScoreString(int score)
    {
        string scoreString = score.ToString("000");
        return scoreString;
    }
}
