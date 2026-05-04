using UnityEngine;
using System.Collections.Generic;

public class MinigameArrowQuadrant : BaseMinigame
{
    [Header("References")]
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Sprite _arrowFilledSprite;

    [Header("Layout")]
    [SerializeField] private int _numberOfArrows = 4;
    [SerializeField] private float _arrowSpacing = 1f;

    [Header("Colors")]
    [SerializeField] private Color _upColor = Color.green;
    [SerializeField] private Color _rightColor = Color.blue;
    [SerializeField] private Color _downColor = Color.red;
    [SerializeField] private Color _leftColor = Color.yellow;

    private SpriteRenderer _playZoneSprite; // sprite on this GameObject used for bounds

    private enum Direction { Up = 0, Right = 1, Down = 2, Left = 3 }

    private List<Direction> _sequence = new List<Direction>();
    private List<GameObject> _spawnedArrows = new List<GameObject>();
    private List<GameObject> _quadrantBackgrounds = new List<GameObject>();
    private int _nextIndex = 0;
    private Sprite _bgSprite;

    protected override void StartMinigame()
    {
        _playZoneSprite = transform.parent.GetComponent<SpriteRenderer>();

        // If the BaseMinigame wasn't initialized externally, fall back to the play-zone sprite bounds
        if (_playZoneSprite != null)
        {
            _boundsCenter = _playZoneSprite.bounds.center;
            _boundsSize = _playZoneSprite.bounds.size;
        }

        if (_arrowPrefab == null)
        {
            Debug.LogError("Arrow prefab not assigned on " + name);
            enabled = false;
            return;
        }

        if (_arrowPrefab.GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogError("Arrow prefab must contain a SpriteRenderer: " + _arrowPrefab.name);
            enabled = false;
            return;
        }

        SpawnArrows();
    }

    protected override void UpdateMinigame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void SpawnArrows()
    {
        ClearExisting();
        _sequence.Clear();
        _spawnedArrows.Clear();
        _nextIndex = 0;

        Vector3 center = new Vector3(_boundsCenter.x, _boundsCenter.y, transform.position.z);
        float availableWidth = (_boundsSize.x > 0f) ? _boundsSize.x : ((_playZoneSprite != null) ? _playZoneSprite.bounds.size.x : (_numberOfArrows - 1) * _arrowSpacing);
        float spacing = (_numberOfArrows > 1) ? Mathf.Min(_arrowSpacing, availableWidth / Mathf.Max(1, _numberOfArrows - 1)) : 0f;
        float totalWidth = spacing * (_numberOfArrows - 1);
        float startX = center.x - totalWidth / 2f;

        for (int i = 0; i < _numberOfArrows; i++)
        {
            Direction dir = (Direction)Random.Range(0, 4);
            _sequence.Add(dir);

            Vector3 pos = new Vector3(startX + i * spacing, center.y + _boundsSize.y / 2.5f, center.z);
            float rot = GetRotationForDirection(dir);
            GameObject go = Instantiate(_arrowPrefab, pos, Quaternion.Euler(0f, 0f, rot), transform);
            go.name = $"Arrow_{i}";

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = GetColorForDirection(dir);
            }

            _spawnedArrows.Add(go);
        }
    } 

    private void ClearExisting()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Arrow_") || child.name.StartsWith("Quadrant_"))
                Destroy(child.gameObject);
        }
        _quadrantBackgrounds.Clear();
    }

    private float GetRotationForDirection(Direction d)
    {
        // assume the arrow sprite faces up by default
        switch (d)
        {
            case Direction.Up: return 0f;
            case Direction.Right: return -90f;
            case Direction.Down: return 180f;
            case Direction.Left: return 90f;
        }
        return 0f;
    }

    private Color GetColorForDirection(Direction d)
    {
        switch (d)
        {
            case Direction.Up: return _upColor;
            case Direction.Right: return _rightColor;
            case Direction.Down: return _downColor;
            case Direction.Left: return _leftColor;
        }
        return Color.white;
    }

    private void HandleClick()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 wp = cam.ScreenToWorldPoint(Input.mousePosition);
        wp.z = transform.position.z;
        Vector3 local = wp - transform.position;

        Direction clicked = GetDirectionFromVector(local);

        if (_nextIndex >= _sequence.Count) return;

        if (clicked == _sequence[_nextIndex])
        {
            // correct
            GameObject hit = _spawnedArrows[_nextIndex];
            if (hit != null)
            {
                // optional: fade/disable
                SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();
                sr.sprite = _arrowFilledSprite;
            }
            _nextIndex++;
            if (_nextIndex >= _sequence.Count)
            {
                WinGame();
            }
            SFXManager.Instance.PlayArrowClick();
        }
        else
        {
            //Debug.Log("Wrong quadrant clicked. Expected " + _sequence[_nextIndex] + " but got " + clicked);
            FailGame();
        }
    }

    private Direction GetDirectionFromVector(Vector3 v)
    {
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
        {
            return (v.x > 0) ? Direction.Right : Direction.Left;
        }
        else
        {
            return (v.y > 0) ? Direction.Up : Direction.Down;
        }
    }

    protected override void ApplyDifficultySettings()
    {
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
            _numberOfArrows = 2;
            _arrowSpacing = 1f;
                break;
            case Difficulty.Normal:
            _numberOfArrows = 4;
            _arrowSpacing = 1f;
                break;
            case Difficulty.Hard:
            _numberOfArrows = 6;
            _arrowSpacing = 1f;
                break;
            case Difficulty.Boss:
            _numberOfArrows = 10;
            _arrowSpacing = 0.75f;
                break;
        }
    }
}
