using UnityEngine;

public class MinigameDragObjectAway : BaseMinigame
{
    [SerializeField] private GameObject _draggablePrefab;
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private float _obstacleSpeed = 3f;
    private GameObject _draggableObject;
    private GameObject _movingObstacle;

    protected override void StartMinigame()
    {
        _draggableObject = Instantiate(_draggablePrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);
        _movingObstacle = Instantiate(_obstaclePrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);

        Rigidbody2D draggableRb = _draggableObject.GetComponent<Rigidbody2D>();
        BoxCollider2D draggableCollider = _draggableObject.GetComponent<BoxCollider2D>();
        Rigidbody2D obstacleRb = _movingObstacle.GetComponent<Rigidbody2D>();
        BoxCollider2D obstacleCollider = _movingObstacle.GetComponent<BoxCollider2D>();
    }

    protected override void UpdateMinigame()
    {
        // Move the obstacle toward the draggable object
        _movingObstacle.transform.position = Vector2.MoveTowards(
            _movingObstacle.transform.position,
            _draggableObject.transform.position,
            Time.deltaTime * _obstacleSpeed
        );

        // Drag the object with the mouse
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _draggableObject.transform.position = mousePosition;
        }

        // Check if the obstacle collides with the draggable object
        if (Vector2.Distance(_draggableObject.transform.position, _movingObstacle.transform.position) < 0.5f)
        {
            FailGame();
        }

        // Check if the draggable object is outside the bounds
        if (!bounds.bounds.Contains(_draggableObject.transform.position))
        {
            WinGame();
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
}