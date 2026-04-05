using UnityEngine;

public class MinigameMovingTarget : BaseMinigame
{
    [SerializeField] private GameObject _movingTargetPrefab;
    [SerializeField] private float _obstacleSpeed = 8f;
    [SerializeField] private GameObject _soulParticlePrefab;
    [SerializeField] private AnimationCurve _speedAnimationCurve;
    private GameObject _movingTarget;
    private Vector2 _currentDirection;
    private float _totalDuration; // Store the initial time to calculate progress
    private Camera _cachedCam;

    protected override void StartMinigame()
    {
        _cachedCam = Camera.main;

        // Capture starting time
        _totalDuration = _gameTimer;

        _movingTarget = Instantiate(_movingTargetPrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);

        // Set an initial random direction
        _currentDirection = GetRandomDirection();
    }

    protected override void UpdateMinigame()
    {
        if (_movingTarget == null) return;

        // Calculate progress (0.0 at start, 1.0 at finish)
        float progress = Mathf.Clamp01((_totalDuration - _gameTimer) / _totalDuration);

        // Move the target in the current direction
        float speedMultiplier = _speedAnimationCurve.Evaluate(progress);
        _movingTarget.transform.position += (Vector3)(_currentDirection * Time.deltaTime * _obstacleSpeed * speedMultiplier);

        // Check if the target hits the bounds and bounce
        Vector2 position = _movingTarget.transform.position;
        Vector2 halfSize = _boundsSize / 2f;

        if (position.x <= _boundsCenter.x - halfSize.x || position.x >= _boundsCenter.x + halfSize.x)
        {
            // Reverse X direction
            _currentDirection.x = -_currentDirection.x;
            position.x = Mathf.Clamp(position.x, _boundsCenter.x - halfSize.x, _boundsCenter.x + halfSize.x);
        }
        if (position.y <= _boundsCenter.y - halfSize.y || position.y >= _boundsCenter.y + halfSize.y)
        {
            // Reverse Y direction
            _currentDirection.y = -_currentDirection.y;
            position.y = Mathf.Clamp(position.y, _boundsCenter.y - halfSize.y, _boundsCenter.y + halfSize.y);
        }

        _movingTarget.transform.position = position;

        // Check for mouse click on the target
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = _cachedCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, MinigameLayerMask);

            if (hit.collider != null && hit.collider.gameObject == _movingTarget)
            {
                if (_soulParticlePrefab != null)
                {
                    Instantiate(_soulParticlePrefab, _movingTarget.transform.position, Quaternion.identity);
                }
                // Destroy the target on click
                Destroy(_movingTarget);
                WinGame();
            }
        }
    }
}