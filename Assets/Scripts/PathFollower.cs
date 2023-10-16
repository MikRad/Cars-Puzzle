using PathCreation;
using UnityEngine;

public abstract class PathFollower : MonoBehaviour
{
    protected enum State { StartPoint, EndPoint, MoveForward, MoveBack, Broken, Disabled}

    [Header("Path follow")]
    [SerializeField] protected PathCreator _pathCreator;
    [SerializeField] private EndOfPathInstruction _endOfPathInstruction;
    [SerializeField] private float _speed = 5;

    protected State _currentState;

    protected Transform _cachedTransform;
    protected float _moveDirectionMultiplier;
    private float _distanceTravelled;

    protected bool IsMoving => (_currentState == State.MoveForward) || (_currentState == State.MoveBack);

    protected void Move()
    {
        if (_pathCreator != null)
        {
            float distance = _pathCreator.path.GetClosestDistanceAlongPath(_cachedTransform.position);

            _distanceTravelled += _speed * Time.deltaTime * _moveDirectionMultiplier;

            _cachedTransform.position = _pathCreator.path.GetPointAtDistance(_distanceTravelled, _endOfPathInstruction);
            _cachedTransform.rotation = _pathCreator.path.GetRotationAtDistance(_distanceTravelled, _endOfPathInstruction);

            if (_currentState == State.MoveForward && distance >= _pathCreator.path.length)
            {
                HandleEndPointReached();
            }
            else if (_currentState == State.MoveBack && distance <= 0f)
            {
                HandleStartPointReached();
            }
        }
    }

    protected void SetState(State newState)
    {
        if (newState == _currentState)
            return;
        
        switch (newState)
        {
            case State.StartPoint:
            case State.EndPoint:
            case State.Broken:
            case State.Disabled:
                _moveDirectionMultiplier = 0f;
                break;
            case State.MoveForward:
                _moveDirectionMultiplier = 1f;
                break;
            case State.MoveBack:
                _moveDirectionMultiplier = -1f;
                break;
        }
        _currentState = newState;
    }

    protected virtual void HandleStartPointReached()
    {
        SetState(State.StartPoint);
    }
    
    protected virtual void HandleEndPointReached()
    {
        SetState(State.EndPoint);
    }
}