using System.Collections.Generic;
using UnityEngine;

public class MinigameRotate : BaseMinigame
{
    [SerializeField] private GameObject _rotatablePrefab;
    [SerializeField] private int _numberOfObjectsToRotate = 3;
    [SerializeField] private float _rotateAmount = 45f;
    [SerializeField] private float _snapThresholdAngle = 10f;
    [SerializeField] private float _artFacingOffset = 0f;

    private readonly List<RotatableObject> _spawned = new List<RotatableObject>();
    private int _completedCount = 0;

    protected override void StartMinigame()
    {
        _completedCount = 0;
        ClearExistingObjects();

        for (int i = 0; i < _numberOfObjectsToRotate; i++)
        {
            Vector2 pos = GetRandomPositionInBounds();
            GameObject go = Instantiate(_rotatablePrefab, pos, Quaternion.identity, transform);
            
            int totalSteps = Mathf.RoundToInt(360f / _rotateAmount);
            int randomStep = Random.Range(0, totalSteps);
            float target = randomStep * _rotateAmount;

            RotatableObject rot = go.GetComponent<RotatableObject>();
            rot._rotateAmount = _rotateAmount;
            rot._snapThresholdAngle = _snapThresholdAngle;
            rot._uprightOffset = _artFacingOffset;

            // Subscribe to both events
            rot.OnCompleted += HandleObjectCompleted;
            rot.OnNotCompleted += HandleObjectUnCompleted;

            // Setup angles
            rot.SetTargetAngle(target);
            rot.RandomizeInitialRotation();

            _spawned.Add(rot);
        }
    }

    private void HandleObjectCompleted(RotatableObject obj)
    {
        _completedCount++;
        CheckWinCondition();
    }

    private void HandleObjectUnCompleted(RotatableObject obj)
    {
        _completedCount--;
    }

    private void CheckWinCondition()
    {
        if (_completedCount >= _spawned.Count)
        {
            WinGame();
        }
    }

    private void ClearExistingObjects()
    {
        foreach (var obj in _spawned)
        {
            if (obj != null) Destroy(obj.gameObject);
        }
        _spawned.Clear();
    }

    protected override void UpdateMinigame() { }
}