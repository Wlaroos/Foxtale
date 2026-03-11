using UnityEngine;

public class MinigameDragObjectAway : BaseMinigame
{
    [SerializeField] private GameObject _draggablePrefab;
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private float _obstacleSpeed = 3f;
    [SerializeField] private float _minSpawnDistance = 3f; // Minimum distance between the two objects
    private GameObject _draggableObject;
    private GameObject _movingObstacle;


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
        var draggableComp = _draggableObject.GetComponent<DraggableObject>();
        if (draggableComp != null)
        {
            draggableComp.SetLayerMask(MinigameLayerMask);
        }
        _movingObstacle = Instantiate(_obstaclePrefab, obstaclePosition, Quaternion.identity, transform);
    }

    protected override void UpdateMinigame()
    {
        // Move the obstacle toward the draggable object
        _movingObstacle.transform.position = Vector2.MoveTowards(
            _movingObstacle.transform.position,
            _draggableObject.transform.position,
            Time.deltaTime * _obstacleSpeed
        );


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

}