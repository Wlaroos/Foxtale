using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

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

        _bottomArea = Instantiate(_bottomAreaPrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);

        for (int i = 0; i < _items; i++)
        {
            // Ensure stackables don't spawn too close to the bottom area
            var pos = GetRandomPositionInBounds();
            while (Vector2.Distance(pos, _bottomArea.transform.position) < 1f)
            {
                pos = GetRandomPositionInBounds();
            }

            _stackables[i] = Instantiate(_stackablePrefab, pos, Quaternion.identity, transform);
        }

        _stacked = new bool[_items];

    }

    protected override void UpdateMinigame()
    {
        // Check if all stackables are close to the bottom area position
        Vector2 bottomAreaPosition = _bottomArea.transform.position;

        if (_stackables == null || _bottomArea == null)
            return;

        bool allStacked = true;

        for (int i = 0; i < _items; i++)
        {
            if (_stackables[i] == null)
            {
                _stacked[i] = false;
                allStacked = false;
                continue;
            }

            if (Vector2.Distance(_stackables[i].transform.position, bottomAreaPosition) < 0.5f)
            {
                if (_stackParticlePrefab != null && _stacked[i] == false)
                {
                    Instantiate(_stackParticlePrefab, bottomAreaPosition, Quaternion.identity);
                }

                _stacked[i] = true;


                var sr = _stackables[i].GetComponent<SpriteRenderer>();
                var script = _stackables[i].GetComponent<DraggableObject>();

                Destroy(script); // Remove the draggable component to prevent further movement

                if (sr != null) sr.color = new Color(0, 1, 0, 0.5f);
                StartCoroutine(HideStackable(sr, 0.25f));
                
                _stackables[i].transform.position = bottomAreaPosition; // Snap to bottom area position
            }
            else
            {
                _stacked[i] = false;
                allStacked = false;
            }
        }


        foreach (bool isStacked in _stacked)
        {
            if (!isStacked)
            {
                allStacked = false;
                break;
            }
        }

        if (allStacked)
        {
            WinGame();
        }

    }

    private IEnumerator HideStackable(SpriteRenderer sr, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sr != null)
        {
            sr.enabled = false;
        }
    }
}
