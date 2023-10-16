using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Car : PathFollower
{
    [Header("Car settings")]
    [SerializeField] private Transform _carBodyTransform;
    [SerializeField] private Vector3 _stopCarEffectRotation = new Vector3(5f, 0f, 0f);
    [SerializeField] private float _stopCarEffectRotationDuration = 0.5f;
    
    private Rigidbody _rBody;
    private BoxCollider _boxCollider;
    private Camera _camera;

    public static event Action<Car> OnCreated;
    public event Action OnStartPointReached;
    public event Action OnEndPointReached;
    public event Action OnBroken;

    private void Start()
    {
        _rBody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();

        _rBody.isKinematic = true;
        _boxCollider.isTrigger = true;
        _cachedTransform = transform;
        _camera = Camera.main;
        
        SetState(State.StartPoint);
        
        OnCreated?.Invoke(this);        
    }

    private void Update()
    {
        UpdateCurrentState();
    }

    public void Disable()
    {
        if (_currentState != State.Broken)
            _currentState = State.Disabled;
    }
    
    private void UpdateCurrentState()
    {
        switch (_currentState)
        {
            case State.StartPoint:
            case State.EndPoint:
                CheckSelection();
                break;
            case State.MoveForward:
            case State.MoveBack:
                Move();
                break;
        }
    }

    private void CheckSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out Car car) && car == this)
                {
                    if (_currentState == State.StartPoint)
                        SetState(State.MoveForward);
                    else if (_currentState == State.EndPoint)
                        SetState(State.MoveBack);
                }
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Car _))
        {
            SetState(State.Broken);
            
            _rBody.isKinematic = false;
            _boxCollider.isTrigger = false;

            AddHitEffect();
            
            OnBroken?.Invoke();
        }
    }

    private void AddHitEffect()
    {
        _rBody.AddForceAtPosition((_cachedTransform.forward + _cachedTransform.up) * 50f, _cachedTransform.position);
    }
    
    private void AddStopEffect()
    {
        if (_currentState == State.EndPoint)
        {
            _carBodyTransform.DOLocalRotate(_stopCarEffectRotation, _stopCarEffectRotationDuration)
                .SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        else if (_currentState == State.StartPoint)
        {
            _carBodyTransform.DOLocalRotate(-_stopCarEffectRotation, _stopCarEffectRotationDuration)
                .SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }
    
    protected override void HandleStartPointReached()
    {
        base.HandleStartPointReached();
        
        AddStopEffect();
        
        OnStartPointReached?.Invoke();
    }

    protected override void HandleEndPointReached()
    {
        base.HandleEndPointReached();
        
        AddStopEffect();
        
        OnEndPointReached?.Invoke();
    }
}
