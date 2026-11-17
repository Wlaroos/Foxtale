using UnityEngine;
using System.Collections.Generic;

public class MinigameImposter : BaseMinigame
{
    [SerializeField] private GameObject _facePrefab;
    [SerializeField] private GameObject _imposterPrefab;
    [SerializeField] private Sprite _pressedSprite;
    [SerializeField] private ParticleSystem _clickedEffect;
    private int _totalFaces = 3;
    private int _totalImposters = 1;
    private GameObject _collectiblesParent;
    private int _imposterClickedCount = 0;

    protected override void StartMinigame()
    {
        _collectiblesParent = new GameObject("FaceParent");
        _collectiblesParent.transform.parent = transform;

        // Get all valid non-touching positions first
        List<Vector2> allSpawnPositions = GetRandomPositionsInBounds(_totalFaces + _totalImposters, 1f);

        int count = 0;
        // Spawn objects at those specific positions
        foreach (Vector2 pos in allSpawnPositions)
        {
            // Spawn imposters first, then faces
            if (count < _totalImposters)
            {
                SpawnClickableObjectAt(pos, _imposterPrefab);
                count++;
            }
            else
            {
                SpawnClickableObjectAt(pos, _facePrefab);
            }
        }
    }

    private void SpawnClickableObjectAt(Vector2 pos, GameObject prefab)
    {
        Instantiate(prefab, pos, Quaternion.identity, _collectiblesParent.transform);
    }

    protected override void UpdateMinigame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, MinigameLayerMask);

            if (hit.collider != null && hit.collider.transform.parent == _collectiblesParent.transform && hit.collider.gameObject.name.Contains("Imposter"))
            {
                hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = _pressedSprite;
                
                Instantiate(_clickedEffect, hit.transform.position, Quaternion.identity);

                _imposterClickedCount++;

                hit.collider.enabled = false;

                if (_imposterClickedCount >= _totalImposters)
                {
                    WinGame();
                }

                SFXManager.Instance.PlayButtonClick();
            }
            else if (hit.collider != null && hit.collider.transform.parent == _collectiblesParent.transform && hit.collider.gameObject.name.Contains("Face"))
            {
                hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = _pressedSprite;
                
                Instantiate(_clickedEffect, hit.transform.position, Quaternion.identity);

                hit.collider.enabled = false;

                FailGame();

                SFXManager.Instance.PlayButtonClick();
            }
        }
    }

    protected override void ApplyDifficultySettings()
    {
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
            _totalFaces = 3;
            _totalImposters = 1;
                break;
            case Difficulty.Normal:
            _totalFaces = 5;
            _totalImposters = 2;
                break;
            case Difficulty.Hard:
            _totalFaces = 7;
            _totalImposters = 3;
                break;
            case Difficulty.Boss:
            _totalFaces = 10;
            _totalImposters = 4;
                break;
        }
    }
}