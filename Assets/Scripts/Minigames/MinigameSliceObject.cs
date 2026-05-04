using System.Collections.Generic;
using UnityEngine;

public class MinigameSliceObject : BaseMinigame
{
    [SerializeField] private GameObject _sliceablePrefab;
    [SerializeField] private int _objectsToSlice = 1;
    [SerializeField] private ParticleSystem _bloodEffect;
    [SerializeField] private TrailRenderer _trailEffect;
    private Vector2 _sliceStart;
    private Vector2 _previousSlicePos;
    private bool _isSlicing;
    private HashSet<int> _slicedThisDrag = new HashSet<int>();
    private TrailRenderer _activeTrail;
    private const float DOT_THRESHOLD = 0.9f;

    protected override void StartMinigame()
    {
        // Get all valid non-touching positions first
        List<Vector2> spawnPositions = GetRandomPositionsInBounds(_objectsToSlice, 1.5f);

        // Spawn objects at those specific positions
        foreach (Vector2 pos in spawnPositions)
        {
            SpawnSliceableObjectAt(pos);
        }

        if (_trailEffect != null)
        {
            _activeTrail = Instantiate(_trailEffect);
            _activeTrail.gameObject.SetActive(false);
            _activeTrail.emitting = false;
            _activeTrail.Clear();
        }
    }

    private void SpawnSliceableObjectAt(Vector2 pos)
    {
        GameObject sliceableObject = Instantiate(_sliceablePrefab, pos, Quaternion.identity, transform);

        Vector2 requiredDirection = GetRandomDirection();
        float angle = Mathf.Atan2(requiredDirection.y, requiredDirection.x) * Mathf.Rad2Deg;
        sliceableObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        var so = sliceableObject.GetComponent<SliceableObject>();
        so.requiredDirection = sliceableObject.transform.right;
    }

    protected override void UpdateMinigame()
    {
        // Start slicing on mouse down
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;
            _sliceStart = worldPos;
            _previousSlicePos = _sliceStart;
            _slicedThisDrag.Clear();
            _isSlicing = true;

            if (_activeTrail != null)
            {
                _activeTrail.emitting = false;
                _activeTrail.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
                _activeTrail.Clear();
                _activeTrail.gameObject.SetActive(true);
                _activeTrail.emitting = true;
            }
        }

        // Linecasts between last and current mouse positions
        if (Input.GetMouseButton(0) && _isSlicing)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;
            Vector2 currentPos = worldPos;

            if (_activeTrail != null)
            {
                _activeTrail.transform.position = new Vector3(currentPos.x, currentPos.y, 0f);
                _activeTrail.emitting = true;
            }

            // Only process if there's actual movement
            if (Vector2.Distance(_previousSlicePos, currentPos) > 0.01f)
            {
                RaycastHit2D[] hits = Physics2D.LinecastAll(_previousSlicePos, currentPos);
                Vector2 segmentDir = (currentPos - _previousSlicePos).normalized;

                foreach (var hit in hits)
                {
                    Collider2D col = hit.collider;
                    if (col == null) continue;

                    // Find the sliceable component on this collider or a parent (handles child colliders)
                    var sliceableObject = col.GetComponentInParent<SliceableObject>();
                    if (sliceableObject == null) continue;

                    GameObject sliceableObj = sliceableObject.gameObject;
                    if (!sliceableObj.CompareTag("Sliceable")) continue;

                    int id = sliceableObj.GetInstanceID();
                    if (_slicedThisDrag.Contains(id)) continue;

                    Vector2 requiredDir = sliceableObject.requiredDirection;

                    if (Vector2.Dot(segmentDir, requiredDir) >= DOT_THRESHOLD)
                    {
                        Instantiate(_bloodEffect, sliceableObj.transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(segmentDir.y, segmentDir.x) * Mathf.Rad2Deg));

                        sliceableObj.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                        sliceableObj.transform.GetChild(0).GetChild(2).parent = null;
                        sliceableObj.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                        sliceableObj.transform.GetChild(0).GetChild(1).parent = null;

                        Destroy(sliceableObj);

                        if (_activeTrail != null) 
                        { 
                            _activeTrail.emitting = false; _activeTrail.Clear(); _activeTrail.gameObject.SetActive(false);
                        }
                        _slicedThisDrag.Add(id);

                        _objectsToSlice--;

                        SFXManager.Instance.PlayBoneBreak();

                        if (_objectsToSlice <= 0)
                        {
                            _isSlicing = false;
                            if (_activeTrail != null) { _activeTrail.emitting = false; _activeTrail.Clear(); _activeTrail.gameObject.SetActive(false); }
                            WinGame();
                            return;
                        }
                    }
                    else
                    {
                        FailGame();
                        _isSlicing = false;
                        if (_activeTrail != null) { _activeTrail.emitting = false; _activeTrail.Clear(); _activeTrail.gameObject.SetActive(false); }
                        return;
                    }
                }
                _previousSlicePos = currentPos;
            }

        }

        // End slicing on mouse up
        if (Input.GetMouseButtonUp(0) && _isSlicing)
        {
            _isSlicing = false;
            if (_activeTrail != null)
            {
                _activeTrail.emitting = false;
                _activeTrail.Clear();
                _activeTrail.gameObject.SetActive(false);
            }
        }
    }

    protected override void ApplyDifficultySettings()
    {
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
            _objectsToSlice = 1;
                break;
            case Difficulty.Normal:
            _objectsToSlice = 3;
                break;
            case Difficulty.Hard:
            _objectsToSlice = 5;
                break;
            case Difficulty.Boss:
            _objectsToSlice = 8;
                break;
        }
    }
}