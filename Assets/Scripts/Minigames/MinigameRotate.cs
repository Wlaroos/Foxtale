using UnityEngine;
using System.Collections.Generic;

public class MinigameRotate : BaseMinigame
{
    [SerializeField] private GameObject _rotatablePrefab;
    [SerializeField] private int _numberOfObjectsToRotate = 2;
    [SerializeField] private float _rotateAmount = 45f;
    [SerializeField] private float _snapThresholdAngle = 10f;
    [SerializeField] private float _artFacingOffset = 0f;

    private readonly List<RotatableObject> _spawned = new List<RotatableObject>();
    private int _completedCount = 0;

    protected override void StartMinigame()
    {
        _completedCount = 0;
        ClearExistingObjects();

        // Get all valid non-touching positions first
        List<Vector2> spawnPositions = GetRandomPositionsInBounds(_numberOfObjectsToRotate, 1.5f);

        // Spawn objects at those specific positions
        foreach (Vector2 pos in spawnPositions)
        {
            SpawnRotatableObjectAt(pos);
        }
    }

    private void SpawnRotatableObjectAt(Vector2 pos)
    {
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

    protected override void ApplyDifficultySettings()
    {
        switch(CurrentDifficulty)
        {
            case Difficulty.Easy:
            _numberOfObjectsToRotate = 1;
            _rotateAmount = 90f;
                break;
            case Difficulty.Normal:
            _numberOfObjectsToRotate = 2;
            _rotateAmount = 45f;
                break;
            case Difficulty.Hard:
            _numberOfObjectsToRotate = 3;
            _rotateAmount = 30f;
                break;
            case Difficulty.Boss:
            _numberOfObjectsToRotate = 5;
            _rotateAmount = 30f;
                break;
        }
    }
}