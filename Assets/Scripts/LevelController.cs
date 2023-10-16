using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Services")]
    [SerializeField] private UIViewsController _uiViewsController;
    
    private enum LevelState { GameProcess, LevelCompleted, LevelFailed}

    private readonly List<Car> _levelCars = new();

    private int _carsReachedEndPoint;
    private int _movesLeft;

    private Level _currentLevel;
    private LevelState _currentLevelState = LevelState.GameProcess;
    
    public event Action OnLevelCompleted; 
    public event Action OnLevelFailed; 
    
    private void Awake()
    {
        Car.OnCreated += HandleCarCreated;
    }

    private void OnDestroy()
    {
        Car.OnCreated -= HandleCarCreated;
    }

    public void StartLevel(Level level)
    {
        Reset();

        _currentLevel = Instantiate(level);
        _movesLeft = _currentLevel.MovesLimit;
        _carsReachedEndPoint = 0;
        
        _uiViewsController.ShowUIView(UIViewType.LevelStatsInfo);
        _uiViewsController.SetLevelStatsInfoMovesLeft(_movesLeft);
    }
    
    private void HandleCarCreated(Car car)
    {
        AddCarEventListeners(car);        
        _levelCars.Add(car);
    }

    private void HandleCarReachedStartPoint()
    {
        _carsReachedEndPoint--;
        
        DecreaseMovesNumber();
    }

    private void HandleCarReachedEndPoint()
    {
        _carsReachedEndPoint++;
        if (_carsReachedEndPoint == _levelCars.Count)
        {
            DisableCars();
            _currentLevelState = LevelState.LevelCompleted;
            
            OnLevelCompleted?.Invoke();
        }
        
        DecreaseMovesNumber();
    }

    private void HandleCarBroken()
    {
        DisableCars();
        _currentLevelState = LevelState.LevelFailed;
        
        OnLevelFailed?.Invoke();
    }

    private void DecreaseMovesNumber()
    {
        _movesLeft--;
        _uiViewsController.SetLevelStatsInfoMovesLeft(_movesLeft);
        
        if (_movesLeft == 0 && _currentLevelState == LevelState.GameProcess)
        {
            DisableCars();
            _currentLevelState = LevelState.LevelFailed;
            
            OnLevelFailed?.Invoke();
        }
    }

    private void AddCarEventListeners(Car car)
    {
        car.OnStartPointReached += HandleCarReachedStartPoint;
        car.OnEndPointReached += HandleCarReachedEndPoint;
        car.OnBroken += HandleCarBroken;
    }
    
    private void RemoveCarEventListeners(Car car)
    {
        car.OnStartPointReached -= HandleCarReachedStartPoint;
        car.OnEndPointReached -= HandleCarReachedEndPoint;
        car.OnBroken -= HandleCarBroken;
    }
    
    private void RemoveCarsEventListeners()
    {
        foreach (Car car in _levelCars)
            RemoveCarEventListeners(car);
    }

    private void DisableCars()
    {
        foreach (Car car in _levelCars)
            car.Disable();
    }

    private void Reset()
    {
        if (_currentLevel == null)
            return;
        
        RemoveCarsEventListeners();
        _levelCars.Clear();
        
        Destroy(_currentLevel.gameObject);
    }
}
