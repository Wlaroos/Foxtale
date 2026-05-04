using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinigameStackItems : BaseMinigame
{
    [SerializeField] private GameObject _stackablePrefab;
    [SerializeField] private GameObject _bottomAreaPrefab;
    [SerializeField] private int _items = 2;
    [SerializeField] private GameObject _stackParticlePrefab;
    
    private GameObject[] _stackables;
    private GameObject _bottomArea;
    private bool[] _stacked;

    protected override void StartMinigame()
    {
        _stackables = new GameObject[_items];
        _stacked = new bool[_items];

        _bottomArea = Instantiate(_bottomAreaPrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);

        // Get all valid non-touching positions first
        List<Vector2> spawnPositions = GetRandomPositionsInBounds(_items, 1.5f);

        for (int i = 0; i < spawnPositions.Count; i++)
        {
            SpawnStackable(spawnPositions[i], i);
        }
    }

    private void SpawnStackable(Vector2 pos, int index)
    {            
        // Ensure stackables don't spawn right on top of the bottom area
        float distanceToBottom = Vector2.Distance(pos, _bottomArea.transform.position);

        if (distanceToBottom < 1.5f) 
        {
            pos = GetRandomPositionInBounds();
        }

        GameObject obj = Instantiate(_stackablePrefab, pos, Quaternion.identity, transform);
        _stackables[index] = obj;

        DraggableObject dragScript = obj.GetComponent<DraggableObject>();
        if (dragScript != null)
        {
            dragScript.SetBounds(_boundsCenter, _boundsSize);
        }
    }

    protected override void UpdateMinigame()
    {
        if (_stackables == null || _bottomArea == null) return;

        Vector2 bottomAreaPosition = _bottomArea.transform.position;
        bool allStacked = true;

        for (int i = 0; i < _items; i++)
        {
            // If it's already stacked and disabled, we just skip it but keep 'allStacked' true
            if (_stacked[i]) continue;

            if (_stackables[i] == null)
            {
                allStacked = false;
                continue;
            }

            if (Vector2.Distance(_stackables[i].transform.position, bottomAreaPosition) < 0.5f)
            {
                StackItem(i, bottomAreaPosition);
            }
            else
            {
                allStacked = false;
            }
        }

        if (allStacked)
        {
            WinGame();
        }
    }

    private void StackItem(int index, Vector2 snapPos)
    {
        _stacked[index] = true;

        if (_stackParticlePrefab != null)
        {
            Instantiate(_stackParticlePrefab, snapPos, Quaternion.identity);
            SFXManager.Instance.PlayButtonClick();
        }

        GameObject item = _stackables[index];
        item.transform.position = snapPos;

        // Visuals and cleanup
        var sr = item.GetComponent<SpriteRenderer>();
        var script = item.GetComponent<DraggableObject>();

        if (script != null) Destroy(script); 
        if (sr != null) sr.color = new Color(0, 1, 0, 0.5f);

        StartCoroutine(HideStackable(item, 0.25f));
    }

    private IEnumerator HideStackable(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {   
            obj.SetActive(false);
        }
    }

    protected override void ApplyDifficultySettings()
    {
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:   
            _items = 1;  
                break;
            case Difficulty.Normal: 
            _items = 3;  
                break;
            case Difficulty.Hard:   
            _items = 5;  
                break;
            case Difficulty.Boss:   
            _items = 10; 
                break;
        }
    }
}