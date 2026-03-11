using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DraggableObject : MonoBehaviour
{
    [SerializeField] private LayerMask _dragLayerMask = ~0;

    private bool _isDragging = false;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
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
