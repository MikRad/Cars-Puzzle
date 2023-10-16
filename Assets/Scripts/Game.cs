using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private Level[] _levelPrefabs;

    [Header("Time delay settings")]
    [SerializeField] private float _levelCompletedDelay = 1f;
    [SerializeField] private float _levelFailedDelay = 1f;

    [Header("Services")]
    [SerializeField] private LevelController _levelController;
    [SerializeField] private UIViewsController _uiViewsController;
    
    private int _currentLevelNumber = 1;
    
    private void Start()
    {
        _levelController.OnLevelCompleted += HandleLevelCompleted;
        _levelController.OnLevelFailed += HandleLevelFailed;
        
        _uiViewsController.AddUIEventSubscriber(UIEventType.LevelCompletedContinueClick, HandleLevelCompletedContinueClick);
        _uiViewsController.AddUIEventSubscriber(UIEventType.LevelFailedPlayAgainClick, HandleLevelFailedPlayAgainClick);
        _uiViewsController.AddUIEventSubscriber(UIEventType.LevelFailedExitClick, ExitApplication);
        
        _levelController.StartLevel(_levelPrefabs[_currentLevelNumber - 1]);        
    }

    private void OnDestroy()
    {
        _levelController.OnLevelCompleted -= HandleLevelCompleted;
        _levelController.OnLevelFailed -= HandleLevelFailed;
        
        _uiViewsController.RemoveUIEventSubscriber(UIEventType.LevelCompletedContinueClick, HandleLevelCompletedContinueClick);
        _uiViewsController.RemoveUIEventSubscriber(UIEventType.LevelFailedPlayAgainClick, HandleLevelFailedPlayAgainClick);
        _uiViewsController.RemoveUIEventSubscriber(UIEventType.LevelFailedExitClick, ExitApplication);
    }

    private void HandleLevelCompleted()
    {
        IncreaseLevelNumber();
        StartCoroutine(UpdateLevelCompletedDelay());
    }
    
    private void HandleLevelFailed()
    {
        StartCoroutine(UpdateLevelFailedDelay());
    }

    private IEnumerator UpdateLevelCompletedDelay()
    {
        yield return new WaitForSeconds(_levelCompletedDelay);
     
        _uiViewsController.ShowUIView(UIViewType.LevelCompletedPanel);
    }
    
    private IEnumerator UpdateLevelFailedDelay()
    {
        yield return new WaitForSeconds(_levelFailedDelay);
        
        _uiViewsController.ShowUIView(UIViewType.LevelFailedPanel);
    }

    private void IncreaseLevelNumber()
    {
        _currentLevelNumber++;
        if (_currentLevelNumber > _levelPrefabs.Length)
        {
            _currentLevelNumber = 1;
        }
    }
    
    private void HandleLevelCompletedContinueClick(object param)
    {
        _levelController.StartLevel(_levelPrefabs[_currentLevelNumber - 1]);        
    }
    
    private void HandleLevelFailedPlayAgainClick(object param)
    {
        _levelController.StartLevel(_levelPrefabs[_currentLevelNumber - 1]);        
    }
    
    private void ExitApplication(object param)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
