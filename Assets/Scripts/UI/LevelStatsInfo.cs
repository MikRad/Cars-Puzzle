using TMPro;
using UnityEngine;

public class LevelStatsInfo : UIView
{
    [SerializeField] private TextMeshProUGUI _movesLeftText;

    public void SetMovesLeft(int movesLeft)
    {
        _movesLeftText.text = $"Moves left: {movesLeft}";
    }
    
    protected override void AddElementsListeners()
    {
    }

    protected override void RemoveElementsListeners()
    {
    }

    protected override void SetEnableElements(bool isEnabled)
    {
    }
}
