using System;
using System.Collections.Generic;
using UnityEngine;

public class UIViewsController : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private Transform _canvasTransform;
    
    [Header("Prefabs")] 
    [SerializeField] private LevelCompletedPanel _levelCompletedPanelPrefab;
    [SerializeField] private LevelFailedPanel _levelFailedPanelPrefab;
    [SerializeField] private LevelStatsInfo _levelStatsInfoPrefab;
    
    private readonly Dictionary<UIViewType, UIView> _uiViewsMap = new();
    private readonly Dictionary<UIEventType, List<Action<object>>> _uiEventsSubscribersMap = new();

    protected void Awake()
    {
        CreateUIViewsMap();
    }

    private void OnEnable()
    {
        AddUIViewsHandlers();
    }

    private void OnDisable()
    {
        RemoveUIViewsHandlers();
    }

    public void ShowUIView(UIViewType viewType)
    {
        _uiViewsMap[viewType].Show();
    }

    public void HideUIView(UIViewType viewType)
    {
        _uiViewsMap[viewType].Hide();
    }

    public void AddUIEventSubscriber(UIEventType eventType, Action<object> eventSubscriber)
    {
        if (_uiEventsSubscribersMap.TryGetValue(eventType, out List<Action<object>> subscribers))
        {
            if (!subscribers.Contains(eventSubscriber))
            {
                subscribers.Add(eventSubscriber);
            }

            return;
        }

        _uiEventsSubscribersMap.Add(eventType, new List<Action<object>>() { eventSubscriber });
    }

    public void RemoveUIEventSubscriber(UIEventType eventType, Action<object> eventSubscriber)
    {
        if (_uiEventsSubscribersMap.TryGetValue(eventType, out List<Action<object>> subscribers))
        {
            if (subscribers.Contains(eventSubscriber))
            {
                subscribers.Remove(eventSubscriber);
            }
        }
    }

    public void SetLevelStatsInfoMovesLeft(int movesLeft)
    {
        LevelStatsInfo levelStatsInfo = _uiViewsMap[UIViewType.LevelStatsInfo] as LevelStatsInfo;
        levelStatsInfo?.SetMovesLeft(movesLeft);
    }
    
    private void AddUIViewsHandlers()
    {
        foreach (KeyValuePair<UIViewType, UIView> mapEntry in _uiViewsMap)
        {
            mapEntry.Value.OnUserEvent += HandleUIViewEvent;
        }
    }
    
    private void RemoveUIViewsHandlers()
    {
        foreach (KeyValuePair<UIViewType, UIView> mapEntry in _uiViewsMap)
        {
            mapEntry.Value.OnUserEvent -= HandleUIViewEvent;
        }
    }

    private void HandleUIViewEvent(UIEventType eventType, object param)
    {
        if (_uiEventsSubscribersMap.TryGetValue(eventType, out List<Action<object>> subscribers))
        {
            NotifySubscribers(subscribers, param);
        }
    }

    private void NotifySubscribers(IEnumerable<Action<object>> subscribers, object param)
    {
        foreach (Action<object> subscriber in subscribers)
        {
            subscriber?.Invoke(param);
        }
    }

    private void CreateUIViewsMap()
    {
        _uiViewsMap.Add(UIViewType.LevelCompletedPanel, Instantiate(_levelCompletedPanelPrefab, _canvasTransform));
        _uiViewsMap.Add(UIViewType.LevelFailedPanel, Instantiate(_levelFailedPanelPrefab, _canvasTransform));
        _uiViewsMap.Add(UIViewType.LevelStatsInfo, Instantiate(_levelStatsInfoPrefab, _canvasTransform));
    }
}
