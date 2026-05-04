using UnityEngine;
using System.Collections.Generic;

public class MinigameBreakObject : BaseMinigame
{
    [SerializeField] private GameObject _breakablePrefab;
    [SerializeField] private int _clicksToBreak = 3;
    [SerializeField] private int _itemsToBreak = 1;
    [SerializeField] private ParticleSystem _breakEffect;
    [SerializeField] private ParticleSystem _boneEffect;
    private int _brokenObjects = 0;

    protected override void StartMinigame()
    {
        // Get all valid non-touching positions first
        List<Vector2> spawnPositions = GetRandomPositionsInBounds(_itemsToBreak, 1f);

        // Spawn objects at those specific positions
        foreach (Vector2 pos in spawnPositions)
        {
            SpawnBreakableObjectAt(pos);
        }
    }

    private void SpawnBreakableObjectAt(Vector2 pos)
    {
        GameObject go = Instantiate(_breakablePrefab, pos, Quaternion.identity, transform);
        BreakableObject br = go.GetComponent<BreakableObject>();

        br.Initialize(_clicksToBreak, _breakEffect, _boneEffect);
        br.Broken += OnBreakableBroken;
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

    protected override void ApplyDifficultySettings()
    {
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
            _itemsToBreak = 1;
            _clicksToBreak = 3;
                break;
            case Difficulty.Normal:
            _itemsToBreak = 2;
            _clicksToBreak = 3;
                break;
            case Difficulty.Hard:
            _itemsToBreak = 3;
            _clicksToBreak = 4;
                break;
            case Difficulty.Boss:
            _itemsToBreak = 4;
            _clicksToBreak = 5;
                break;
        }
    }
}