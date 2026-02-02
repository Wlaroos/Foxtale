using UnityEngine;

public class MinigameBreakObject : BaseMinigame
{
    [SerializeField] private GameObject _breakablePrefab;
    [SerializeField] private int _clicksToBreak = 5;
    [SerializeField] private ParticleSystem _breakEffect;
    [SerializeField] private ParticleSystem _boneEffect;
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
                Instantiate(_boneEffect, hit.transform.position, Quaternion.identity);

                if (_breakClicks >= _clicksToBreak)
                {
                    Instantiate(_breakEffect, hit.transform.position, Quaternion.identity);
                    Destroy(_breakableObject);
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