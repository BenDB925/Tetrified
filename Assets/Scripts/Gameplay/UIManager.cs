using Tetrified.Scripts.Utility;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private UIDocument _rootDocument;

    private TextElement _pointsText;
    private const string PointsTextName = "PointsValue";

    private void Start()
    {
        VisualElement rootElement = _rootDocument.rootVisualElement;
        _pointsText = rootElement.Query<TextElement>(PointsTextName);
    }

    public void SetScore(int score)
    {
        string scoreString = score.ToString();
        if (score < 100)
        {
            scoreString.Insert(0, "0");
        }
        if (score < 10)
        {
            scoreString.Insert(0, "0");
        }

        _pointsText.text = score.ToString();
    }
}
