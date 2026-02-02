using UnityEngine;

public class MinigameCollectItems : BaseMinigame
{
    [SerializeField] private GameObject _collectiblePrefab;
    [SerializeField] private int _totalCollectibles = 5;
    [SerializeField] private Sprite _pressedSprite;
    [SerializeField] private ParticleSystem _clickedEffect;
    private GameObject _collectiblesParent;
    private int _collectedCount = 0;

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
                hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = _pressedSprite;
                
                Instantiate(_clickedEffect, hit.transform.position, Quaternion.identity);

                _collectedCount++;

                hit.collider.enabled = false;

                if (_collectedCount >= _totalCollectibles)
                {
                    WinGame();
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
}