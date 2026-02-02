using UnityEngine;

public class MinigameMovingTarget : BaseMinigame
{
    [SerializeField] private GameObject _movingTargetPrefab;
    [SerializeField] private float _obstacleSpeed = 5f;
    private GameObject _movingTarget;
    private Rigidbody2D _targetRigidbody;
    private Vector2 _currentDirection;

    protected override void StartMinigame()
    {
        _movingTarget = Instantiate(_movingTargetPrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);

        _targetRigidbody = _movingTarget.GetComponent<Rigidbody2D>();
        BoxCollider2D collider = _movingTarget.GetComponent<BoxCollider2D>();

        // Set an initial random direction
        _currentDirection = GetRandomDirection();
    }

    protected override void UpdateMinigame()
    {
        // Move the target in the current direction
        _movingTarget.transform.position += (Vector3)(_currentDirection * Time.deltaTime * _obstacleSpeed);

        // Check if the target hits the bounds and bounce
        Vector2 position = _movingTarget.transform.position;
        if (position.x <= bounds.bounds.min.x || position.x >= bounds.bounds.max.x)
        {
            _currentDirection.x = -_currentDirection.x; // Reverse X direction
        }
        if (position.y <= bounds.bounds.min.y || position.y >= bounds.bounds.max.y)
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
                Destroy(_movingTarget); // Destroy the target on click
                WinGame();
            }
        }
    }

    private Vector2 GetRandomPositionInBounds()
    {
        Vector2 min = bounds.bounds.min;
        Vector2 max = bounds.bounds.max;

        return new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y)
        );
    }

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }
}