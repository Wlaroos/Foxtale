using UnityEngine;

public class MinigameMovingTarget : BaseMinigame
{
    [SerializeField] private GameObject _movingTargetPrefab;
    [SerializeField] private float _obstacleSpeed = 5f;
    [SerializeField] private GameObject _soulParticlePrefab;
    private GameObject _movingTarget;
    private Vector2 _currentDirection;

    protected override void StartMinigame()
    {
        _movingTarget = Instantiate(_movingTargetPrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);

        // Set an initial random direction
        _currentDirection = GetRandomDirection();
    }

    protected override void UpdateMinigame()
    {
        // Move the target in the current direction
        _movingTarget.transform.position += (Vector3)(_currentDirection * Time.deltaTime * _obstacleSpeed);

        // Check if the target hits the bounds and bounce
        Vector2 position = _movingTarget.transform.position;
        if (position.x <= boundsCenter.x - boundsSize.x / 2 || position.x >= boundsCenter.x + boundsSize.x / 2)
        {
            _currentDirection.x = -_currentDirection.x; // Reverse X direction
        }
        if (position.y <= boundsCenter.y - boundsSize.y / 2 || position.y >= boundsCenter.y + boundsSize.y / 2)
        {
            _currentDirection.y = -_currentDirection.y; // Reverse Y direction
        }

        // Check for mouse click on the target
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, MinigameLayerMask);

            if (hit.collider != null && hit.collider.gameObject == _movingTarget)
            {
                if (_soulParticlePrefab != null)
                {
                    Instantiate(_soulParticlePrefab, _movingTarget.transform.position, Quaternion.identity);
                }
                Destroy(_movingTarget); // Destroy the target on click
                WinGame();
            }
        }
    }
}