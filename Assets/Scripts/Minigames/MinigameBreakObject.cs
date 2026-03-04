using UnityEngine;

public class MinigameBreakObject : BaseMinigame
{
    [SerializeField] private GameObject _breakablePrefab;
    [SerializeField] private int _clicksToBreak = 3;
    [SerializeField] private int _itemsToBreak = 1;
    [SerializeField] private ParticleSystem _breakEffect;
    [SerializeField] private ParticleSystem _boneEffect;

    private BreakableObject[] _breakableObjects;
    private int _brokenObjects = 0;

    protected override void StartMinigame()
    {
        _breakableObjects = new BreakableObject[_itemsToBreak];
        for (int i = 0; i < _itemsToBreak; i++)
        {
            GameObject go = Instantiate(_breakablePrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);
            BreakableObject br = go.GetComponent<BreakableObject>();

            br.Initialize(_clicksToBreak, _breakEffect, _boneEffect);
            br.Broken += OnBreakableBroken;

            _breakableObjects[i] = br;
        }
    }

    protected override void UpdateMinigame()
    {
        // Click events are handled by the BreakableObject instances
    }

    private void OnBreakableBroken(BreakableObject broken)
    {
        _brokenObjects++;

        if (_brokenObjects >= _itemsToBreak)
        {
            WinGame();
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