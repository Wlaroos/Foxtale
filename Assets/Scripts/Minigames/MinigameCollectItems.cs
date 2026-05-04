using UnityEngine;
using System.Collections.Generic;

public class MinigameCollectItems : BaseMinigame
{
    [SerializeField] private GameObject _collectiblePrefab;
    [SerializeField] private int _totalCollectibles = 3;
    [SerializeField] private Sprite _pressedSprite;
    [SerializeField] private ParticleSystem _clickedEffect;
    private GameObject _collectiblesParent;
    private int _collectedCount = 0;

    protected override void StartMinigame()
    {
        _collectiblesParent = new GameObject("CollectiblesParent");
        _collectiblesParent.transform.parent = transform;

        // Get all valid non-touching positions first
        List<Vector2> spawnPositions = GetRandomPositionsInBounds(_totalCollectibles, 1f);

        // Spawn objects at those specific positions
        foreach (Vector2 pos in spawnPositions)
        {
            SpawnClickableObjectAt(pos);
        }
    }

    private void SpawnClickableObjectAt(Vector2 pos)
    {
        Instantiate(_collectiblePrefab, pos, Quaternion.identity, _collectiblesParent.transform);
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

                SFXManager.Instance.PlayButtonClick();
            }
        }
    }

    protected override void ApplyDifficultySettings()
    {
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
            _totalCollectibles = 1;
                break;
            case Difficulty.Normal:
            _totalCollectibles = 3;
                break;
            case Difficulty.Hard:
            _totalCollectibles = 5;
                break;
            case Difficulty.Boss:
            _totalCollectibles = 8;
                break;
        }
    }
}