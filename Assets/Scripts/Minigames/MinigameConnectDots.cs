using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectDotsMinigame : BaseMinigame
{
    [Header("Settings")]
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private GameObject _lineSegmentPrefab;
    [SerializeField] private GameObject _jointPrefab;
    [SerializeField] private int _dotCount = 5;
    [SerializeField] private float _dotRadius = 0.5f;
    [SerializeField] private float _lineWidth = 0.1f;
    
    [Header("Visuals")]
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _completedColor = Color.green;
    [SerializeField] private Color _nextColor = Color.yellow;
    [SerializeField] private GameObject _dotConnectParticlePrefab;

    private List<GameObject> _spawnedDots = new List<GameObject>();
    private List<SpriteRenderer> _dotRenderers = new List<SpriteRenderer>();
    private List<GameObject> _activeVisuals = new List<GameObject>();
    
    private GameObject _cursorSegment; 
    private GameObject _cursorJoint; // Extra joint that follows the mouse
    
    private int _currentIndex = 0;
    private bool _isDragging = false;

    protected override void StartMinigame()
    {
        _currentIndex = 0;
        _isDragging = false;
        
        // Setup Cursor Visuals
        if (_cursorSegment == null) _cursorSegment = Instantiate(_lineSegmentPrefab, transform);
        if (_cursorJoint == null) _cursorJoint = Instantiate(_jointPrefab, transform);
        
        _cursorSegment.SetActive(false);
        _cursorJoint.SetActive(false);

        SpawnDotsSequentially();
    }

    private void SpawnDotsSequentially()
    {
        ClearBoard();

        float minRequiredDistance = (_dotRadius * 2f) + 0.5f;
        List<Vector2> positions = GetRandomPositionsInBounds(_dotCount, minRequiredDistance);

        // Iterate through the generated positions to spawn dots
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2 spawnPos = positions[i];
            GameObject dot = Instantiate(_dotPrefab, spawnPos, Quaternion.identity, transform);

            var dotItem = dot.GetComponent<DotItem>();
            if (dotItem != null) dotItem.Index = i;

            TMP_Text text = dot.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = (i + 1).ToString();

            SpriteRenderer sr = dot.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = _defaultColor;
                _dotRenderers.Add(sr);
            }

            _spawnedDots.Add(dot);
        }

        // Update the visual state (highlights the first dot)
        UpdateDotVisuals();
    }

    protected override void UpdateMinigame()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            if (_spawnedDots.Count > 0 && Vector2.Distance(mouseWorldPos, _spawnedDots[0].transform.position) <= _dotRadius)
            {
                _isDragging = true;
                _cursorSegment.SetActive(true);
                _cursorJoint.SetActive(true);
                
                // Add the first permanent joint at Dot 1
                SpawnPermanentJoint(_spawnedDots[0].transform.position);
                
                ConnectToDot(_spawnedDots[0], 0);
            }
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            UpdateVisuals(mouseWorldPos);
            CheckForDotConnection(mouseWorldPos);
        }

        if (Input.GetMouseButtonUp(0)) ResetProgress();
    }

    private void UpdateVisuals(Vector2 mousePos)
    {
        if (_currentIndex > 0 && _isDragging)
        {
            Vector2 startPos = _spawnedDots[_currentIndex - 1].transform.position;
            PositionSegment(_cursorSegment, startPos, mousePos);
            
            // Move the floating joint to the mouse position to cap the end of the line
            _cursorJoint.transform.position = mousePos;
            _cursorJoint.transform.localScale = new Vector3(_lineWidth, _lineWidth, 1f);
        }
    }

    private void CheckForDotConnection(Vector2 currentPos)
    {
        if (_currentIndex >= _spawnedDots.Count) return;

        for (int i = 0; i < _spawnedDots.Count; i++)
        {
            float distance = Vector2.Distance(currentPos, _spawnedDots[i].transform.position);
            if (distance <= _dotRadius)
            {
                if (i == _currentIndex)
                {
                    // Create segment
                    GameObject seg = Instantiate(_lineSegmentPrefab, transform);
                    PositionSegment(seg, _spawnedDots[_currentIndex - 1].transform.position, _spawnedDots[i].transform.position);
                    _activeVisuals.Add(seg);

                    // Joint at the new dot to hide the connection gap
                    SpawnPermanentJoint(_spawnedDots[i].transform.position);

                    ConnectToDot(_spawnedDots[i], i);
                    return;
                }
                else if (i > _currentIndex)
                {
                    FailGame();
                    return;
                }
            }
        }
    }

    private void SpawnPermanentJoint(Vector2 pos)
    {
        GameObject joint = Instantiate(_jointPrefab, pos, Quaternion.Euler(0,0,45), transform);
        _activeVisuals.Add(joint);
    }

    private void PositionSegment(GameObject segment, Vector2 start, Vector2 end)
    {
        Vector2 dir = end - start;
        segment.transform.position = start + (dir / 2f);
        segment.transform.right = dir;
        segment.transform.localScale = new Vector3(dir.magnitude, _lineWidth, 1f);
    }

    private void ConnectToDot(GameObject hitObj, int hitIndex)
    {
        _dotRenderers[hitIndex].color = _completedColor;

        Instantiate(_dotConnectParticlePrefab, hitObj.transform.position, Quaternion.identity);

        SFXManager.Instance.PlayBoneClick();

        _currentIndex++;

        UpdateDotVisuals(); // Update colors for the new state

        if (_currentIndex >= _spawnedDots.Count)
        {
            _cursorSegment.SetActive(false);
            _cursorJoint.SetActive(false);
            WinGame();
            _isDragging = false;
        }
    }

    private void ResetProgress()
    {
        if (!_gameActive) return;
        _isDragging = false;
        _currentIndex = 0;
        _cursorSegment.SetActive(false);
        _cursorJoint.SetActive(false);
        foreach (var sr in _dotRenderers) if (sr != null) sr.color = _defaultColor;
        foreach (var v in _activeVisuals) Destroy(v);
        _activeVisuals.Clear();

        UpdateDotVisuals(); // Reset the first dot to yellow and others to default
    }

    private void ClearBoard()
    {
        foreach (var dot in _spawnedDots) if(dot != null) Destroy(dot);
        foreach (var v in _activeVisuals) if(v != null) Destroy(v);
        _spawnedDots.Clear();
        _dotRenderers.Clear();
        _activeVisuals.Clear();
    }

    private void UpdateDotVisuals()
    {
        for (int i = 0; i < _dotRenderers.Count; i++)
        {
            if (i < _currentIndex)
            {
                _dotRenderers[i].color = _completedColor;
            }
            else if (i == _currentIndex)
            {
                _dotRenderers[i].color = _nextColor;
            }
            else
            {
                _dotRenderers[i].color = _defaultColor;
            }
        }
    }

    protected override void ApplyDifficultySettings()
    {
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
            _dotCount = 2;
                break;
            case Difficulty.Normal:
            _dotCount = 4;
                break;
            case Difficulty.Hard:
            _dotCount = 7;
                break;
            case Difficulty.Boss:
            _dotCount = 10;
                break;
        }
    }
}