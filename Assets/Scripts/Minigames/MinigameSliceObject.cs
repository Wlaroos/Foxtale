using UnityEngine;

public class MinigameSliceObject : BaseMinigame
{
    [SerializeField] private GameObject _sliceablePrefab;
    [SerializeField] private int _objectsToSlice = 2;
    [SerializeField] private ParticleSystem _bloodEffect;
    private Vector2 _requiredSliceDirection;
    private Vector2 _sliceStart;
    private Vector2 _sliceEnd;
    private bool _isSlicing;

    protected override void StartMinigame()
    {
        for (int i = 0; i < _objectsToSlice; i++)
        {
            SpawnSliceableObject();
        }
    }

    private void SpawnSliceableObject()
    {
        GameObject sliceableObject = Instantiate(_sliceablePrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);

        Rigidbody2D rb = sliceableObject.GetComponent<Rigidbody2D>();
        
        BoxCollider2D collider = sliceableObject.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = sliceableObject.GetComponentInChildren<BoxCollider2D>();
        }

        // Set the required slice direction and rotate the object accordingly
        _requiredSliceDirection = GetRandomDirection();
        float angle = Mathf.Atan2(_requiredSliceDirection.y, _requiredSliceDirection.x) * Mathf.Rad2Deg;
        sliceableObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void UpdateMinigame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _sliceStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _isSlicing = true;
        }

        if (Input.GetMouseButtonUp(0) && _isSlicing)
        {
            _sliceEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _isSlicing = false;

            Vector2 sliceDirection = (_sliceEnd - _sliceStart).normalized;

            // Check for slicing interactions
            Collider2D[] hits = Physics2D.OverlapAreaAll(_sliceStart, _sliceEnd);
            foreach (Collider2D hit in hits)
            {
                // Ensure the hit object is a sliceable object
                if (hit != null && hit.CompareTag("Sliceable") && Vector2.Dot(sliceDirection, _requiredSliceDirection) > 0.8f)
                {
                    Instantiate(_bloodEffect, hit.transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(sliceDirection.y, sliceDirection.x) * Mathf.Rad2Deg));

                    Destroy(hit.gameObject);

                    _objectsToSlice--;

                    if (_objectsToSlice <= 0)
                    {
                        WinGame();
                    }
                }
                else if (hit != null && hit.CompareTag("Sliceable"))
                {
                    FailGame();
                }
            }
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

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }
}