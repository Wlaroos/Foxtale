using UnityEngine;

public class MinigameDragObjectAway : BaseMinigame
{
    [SerializeField] private GameObject _draggablePrefab;
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private float _obstacleSpeed = 3f;
    [SerializeField] private float _minSpawnDistance = 3f; // Minimum distance between the two objects
    private GameObject _draggableObject;
    private GameObject _movingObstacle;

    private bool _isDragging = false;

    protected override void StartMinigame()
    {
        Vector2 draggablePosition;
        Vector2 obstaclePosition;

        // Ensure the objects spawn far enough apart
        do
        {
            draggablePosition = transform.position;
            obstaclePosition = GetRandomPositionInBounds();
        } while (Vector2.Distance(draggablePosition, obstaclePosition) < _minSpawnDistance);

        _draggableObject = Instantiate(_draggablePrefab, draggablePosition, Quaternion.identity, transform);
        _movingObstacle = Instantiate(_obstaclePrefab, obstaclePosition, Quaternion.identity, transform);

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

        // Handle dragging logic
        HandleDragging();

        // Check if the obstacle collides with the draggable object
        if (Vector2.Distance(_draggableObject.transform.position, _movingObstacle.transform.position) < 0.5f)
        {
            FailGame();
        }

        // Check if the draggable object is outside the bounds
        if (!new Rect(boundsCenter - boundsSize / 2, boundsSize).Contains(_draggableObject.transform.position))
        {
            WinGame();
        }
    }

    private void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, MinigameLayerMask);

            if (hit.collider != null && hit.collider.gameObject == _draggableObject)
            {
                _isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        if (_isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _draggableObject.transform.position = mousePosition;
        }
    }

    private Vector2 GetRandomPositionInBounds()
    {
        Vector2 min = boundsCenter - boundsSize / 2;
        Vector2 max = boundsCenter + boundsSize / 2;
        return new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y)
        );
    }
}