using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RotatableObject : MonoBehaviour
{
    [HideInInspector] public float _rotateAmount = 45f;
    [HideInInspector] public float _snapThresholdAngle = 10f;
    [HideInInspector] public bool _isInteractable = true;
    [HideInInspector] public float _uprightOffset = 0f; 

    private float _targetAngle = 0f; // Logical target (0, 90, 180...)
    private bool _completed = false;
    [HideInInspector] public bool IsCompleted => _completed;
    private Transform _bodyTransform;
    private Transform _rotatorTransform; 
    private Collider2D _clickCollider;

    public event Action<RotatableObject> OnCompleted;
    public event Action<RotatableObject> OnNotCompleted;

    void Awake() => AssignReferences();

    private void AssignReferences()
    {
        if (_bodyTransform == null && transform.childCount > 0)
            _bodyTransform = transform.GetChild(0);

        if (_rotatorTransform == null && transform.childCount > 1)
            _rotatorTransform = transform.GetChild(1);

        if (_rotatorTransform != null) _clickCollider = _rotatorTransform.GetComponent<Collider2D>();
        if (_clickCollider == null) _clickCollider = GetComponent<Collider2D>();
    }

 public void SetTargetAngle(float angleDegrees)
{
    AssignReferences();
    
    float snappedLogical = RoundToMultiple(angleDegrees, _rotateAmount);
    _targetAngle = NormalizeAngle(snappedLogical);

    if (_bodyTransform != null)
    {
        float visualGoalZ = NormalizeAngle(_targetAngle - _uprightOffset);
        _bodyTransform.localEulerAngles = new Vector3(0f, 0f, visualGoalZ);
    }
    
    UpdateVisualState();
}

public void RandomizeInitialRotation()
{
    AssignReferences();
    if (_rotatorTransform == null) return;

    int totalSteps = Mathf.RoundToInt(360f / _rotateAmount);
    if (totalSteps <= 1) totalSteps = 4;

    int targetStepIndex = Mathf.RoundToInt(NormalizeAngle(_targetAngle) / _rotateAmount) % totalSteps;

    int startStepIndex = UnityEngine.Random.Range(0, totalSteps);
    
    if (startStepIndex == targetStepIndex)
    {
        startStepIndex = (startStepIndex + UnityEngine.Random.Range(1, totalSteps)) % totalSteps;
    }

    float physicalStartAngle = NormalizeAngle((startStepIndex * _rotateAmount) - _uprightOffset);
    _rotatorTransform.localEulerAngles = new Vector3(0f, 0f, physicalStartAngle);
    
    _completed = false;
    UpdateVisualState();
}

    void Update()
    {
        if (!_isInteractable) return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (_clickCollider != null && _clickCollider.OverlapPoint(mousePos))
            {
                RotatePlayerObject(Input.GetMouseButtonDown(0));
            }
        }
    }

    private void RotatePlayerObject(bool clockwise)
    {
        if (_rotatorTransform == null) return;

        float dir = clockwise ? -_rotateAmount : _rotateAmount;
        _rotatorTransform.Rotate(0f, 0f, dir);

        // SNAP TO OFFSET GRID:
        // 1. Get current physical, 2. Add offset to get 'logical' view, 3. Round, 4. Subtract offset
        float currentPhysical = _rotatorTransform.localEulerAngles.z;
        float logicalEquivalent = currentPhysical + _uprightOffset;
        float snappedLogical = RoundToMultiple(logicalEquivalent, _rotateAmount);
        float snappedPhysical = NormalizeAngle(snappedLogical - _uprightOffset);

        _rotatorTransform.localEulerAngles = new Vector3(0f, 0f, snappedPhysical);

        // Check completion: Logical vs Logical
        float delta = Mathf.Abs(Mathf.DeltaAngle(snappedLogical, _targetAngle));
        bool currentlyAtTarget = delta <= _snapThresholdAngle;

        if (currentlyAtTarget && !_completed)
        {
            _completed = true;
            OnCompleted?.Invoke(this);
        }
        else if (!currentlyAtTarget && _completed)
        {
            _completed = false;
            OnNotCompleted?.Invoke(this);
        }

        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        Color stateColor = _completed ? Color.green : Color.white;
        SetSpriteColor(_bodyTransform, stateColor);
        SetSpriteColor(_rotatorTransform, stateColor);
    }

    private void SetSpriteColor(Transform t, Color c)
    {
        if (t == null) return;
        SpriteRenderer sr = t.GetComponent<SpriteRenderer>() ?? t.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.color = c;
    }

    private float RoundToMultiple(float angle, float step) => Mathf.Round(angle / step) * step;
    private float NormalizeAngle(float a) => (a % 360f + 360f) % 360f;
}