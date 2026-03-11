using UnityEngine;

public class MinigameStackItems : BaseMinigame
{
    [SerializeField] private GameObject _stackablePrefab;
    [SerializeField] private GameObject _bottomAreaPrefab;
    [SerializeField] private int _items = 2;
    private GameObject[] _stackables;
    private GameObject _bottomArea;

    protected override void StartMinigame()
    {
        _stackables = new GameObject[_items];
        for (int i = 0; i < _items; i++)
        {
            _stackables[i] = Instantiate(_stackablePrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);
        }

        _bottomArea = Instantiate(_bottomAreaPrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);
    }

    protected override void UpdateMinigame()
    {
        // Check if all stackables are close to the bottom area position
        Vector2 bottomAreaPosition = _bottomArea.transform.position;

        bool[] stacked = new bool[_items];

        bool allStacked = true;

        for (int i = 0; i < _items; i++)
        {
            if (Vector2.Distance(_stackables[i].transform.position, bottomAreaPosition) < 0.5f)
            {
                stacked[i] = true;
            }
            else
            {
                stacked[i] = false;
            }
        }


        foreach (bool isStacked in stacked)
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
}
