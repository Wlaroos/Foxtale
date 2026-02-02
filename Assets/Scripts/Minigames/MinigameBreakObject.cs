using UnityEngine;

public class MinigameBreakObject : BaseMinigame
{
    [SerializeField] private GameObject _breakablePrefab;
    [SerializeField] private int _clicksToBreak = 5;
    private int _breakClicks = 0;
    private GameObject _breakableObject;

    protected override void StartMinigame()
    {
        _breakableObject = Instantiate(_breakablePrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);
        _breakClicks = 0;

        Rigidbody2D rb = _breakableObject.GetComponent<Rigidbody2D>();
        BoxCollider2D collider = _breakableObject.GetComponent<BoxCollider2D>();
    }

    protected override void UpdateMinigame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, MinigameLayerMask);

            if (hit.collider != null && hit.collider.gameObject == _breakableObject)
            {
                _breakClicks++;
                Debug.Log($"Object clicked {_breakClicks} times!");

                if (_breakClicks >= _clicksToBreak)
                {
                    Destroy(_breakableObject);
                    WinGame();
                }
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
}