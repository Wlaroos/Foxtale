using UnityEngine;

public class MinigameCollectItems : BaseMinigame
{
    [SerializeField] private GameObject _collectiblePrefab;
    [SerializeField] private int _totalCollectibles = 5;
    private GameObject _collectiblesParent;

    protected override void StartMinigame()
    {
        _collectiblesParent = new GameObject("CollectiblesParent");
        _collectiblesParent.transform.parent = transform;

        for (int i = 0; i < _totalCollectibles; i++)
        {
            GameObject collectible = Instantiate(_collectiblePrefab, GetRandomPositionInBounds(), Quaternion.identity, _collectiblesParent.transform);

            Rigidbody2D rb = collectible.GetComponent<Rigidbody2D>();
            BoxCollider2D collider = collectible.GetComponent<BoxCollider2D>();
        }
    }

    protected override void UpdateMinigame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, MinigameLayerMask);

            if (hit.collider != null && hit.collider.transform.parent == _collectiblesParent.transform)
            {
                Destroy(hit.collider.gameObject);

                if (_collectiblesParent.transform.childCount == 0)
                {
                    WinGame();
                }
            }
        }

        if (Input.GetMouseButton(0)) // Detect slicing motion
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] hits = Physics2D.OverlapCircleAll(mousePosition, 0.5f);

            foreach (Collider2D hit in hits)
            {
                if (hit.transform.parent == _collectiblesParent.transform)
                {
                    Destroy(hit.gameObject);
                }
            }

            if (_collectiblesParent.transform.childCount == 0)
            {
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
}