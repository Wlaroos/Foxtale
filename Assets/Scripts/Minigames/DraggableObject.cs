using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DraggableObject : MonoBehaviour
{
    [SerializeField] private LayerMask _dragLayerMask = ~0;

    private bool _isDragging = false;
    private Camera _mainCamera;

    // Boundary variables
    private Vector2 _minBounds;
    private Vector2 _maxBounds;
    private bool _useBounds = false;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    /// Call this from the Minigame script to restrict movement area.
    public void SetBounds(Vector2 center, Vector2 size)
    {
        _minBounds = center - size / 2f;
        _maxBounds = center + size / 2f;
        _useBounds = true;
    }

    private void Update()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, _dragLayerMask);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
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
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            if (_useBounds)
            {
                // Clamp the mouse position within the specified bounds
                mousePosition.x = Mathf.Clamp(mousePosition.x, _minBounds.x, _maxBounds.x);
                mousePosition.y = Mathf.Clamp(mousePosition.y, _minBounds.y, _maxBounds.y);
            }

            transform.position = mousePosition;
        }
    }

    private void OnDisable()
    {
        _isDragging = false;
    }

    public void SetLayerMask(LayerMask mask)
    {
        _dragLayerMask = mask;
    }
}