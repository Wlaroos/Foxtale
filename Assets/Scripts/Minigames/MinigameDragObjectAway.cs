using UnityEngine;
using System.Collections.Generic;

public class MinigameDragObjectAway : BaseMinigame
{
    [SerializeField] private GameObject _draggablePrefab;
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private int _numberOfObstacles = 1;
    [SerializeField] private float _obstacleSpeed = 3f;
    [SerializeField] private float _minSpawnDistance = 3f; // Minimum distance between the two objects
    [SerializeField] private GameObject _escapeParticlePrefab;
    private GameObject _draggableObject;
    private List<GameObject> _movingObstacles = new List<GameObject>();


    protected override void StartMinigame()
    {
        // 1. Spawn the draggable object first
        _draggableObject = Instantiate(_draggablePrefab, transform.position, Quaternion.identity, transform);
        
        var draggableComp = _draggableObject.GetComponent<DraggableObject>();
        if (draggableComp != null)
        {
            draggableComp.SetLayerMask(MinigameLayerMask);
        }

        // 2. Spawn multiple obstacles based on difficulty settings
        for (int i = 0; i < _numberOfObstacles; i++)
        {
            Vector2 obstaclePosition;
            int attempts = 0;

            // Ensure each obstacle is far enough from the player
            do
            {
                obstaclePosition = GetRandomPositionInBounds();
                attempts++;
            } 
            while (Vector2.Distance(_draggableObject.transform.position, obstaclePosition) < _minSpawnDistance && attempts < 100);

            GameObject newObstacle = Instantiate(_obstaclePrefab, obstaclePosition, Quaternion.identity, transform);
            _movingObstacles.Add(newObstacle);
        }
    }

    protected override void UpdateMinigame()
    {
        if (_draggableObject == null) return;

        // Loop through all active obstacles
        foreach (GameObject obstacle in _movingObstacles)
        {
            if (obstacle == null) continue;

            // Move the obstacle toward the draggable object
            obstacle.transform.position = Vector2.MoveTowards(
                obstacle.transform.position,
                _draggableObject.transform.position,
                Time.deltaTime * _obstacleSpeed
            );

            // Check for collision with any obstacle
            if (Vector2.Distance(_draggableObject.transform.position, obstacle.transform.position) < 0.5f)
            {
                SFXManager.Instance.PlayCat2();
                SFXManager.Instance.PlayAttack();
                FailGame();
                return; // Exit early if we already failed
            }
        }

        // Check if the draggable object is outside the bounds (Win Condition)
        Rect bounds = new Rect(_boundsCenter - _boundsSize / 2, _boundsSize);
        if (!bounds.Contains(_draggableObject.transform.position))
        {
            HandleWin();
        }
    }

    private void HandleWin()
    {
        if (_escapeParticlePrefab != null)
        {
            // Point particles away from the first obstacle (or center)
            Vector2 effectSource = _movingObstacles.Count > 0 ? _movingObstacles[0].transform.position : transform.position;
            var dir = (Vector2)_draggableObject.transform.position - effectSource;
            float rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            
            Instantiate(_escapeParticlePrefab, _draggableObject.transform.position, Quaternion.Euler(0, 0, rotation));
            SFXManager.Instance.PlayCat();
        }
        WinGame();
    }

    protected override void ApplyDifficultySettings()
    {
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
            _numberOfObstacles = 1;
            _obstacleSpeed = 1f;
                break;
            case Difficulty.Normal:
            _numberOfObstacles = 1;
            _obstacleSpeed = 3f;
                break;
            case Difficulty.Hard:
            _numberOfObstacles = 2;
            _obstacleSpeed = 3f;
                break;
            case Difficulty.Boss:
            _numberOfObstacles = 3;
            _obstacleSpeed = 4f;
                break;
        }
    }
}