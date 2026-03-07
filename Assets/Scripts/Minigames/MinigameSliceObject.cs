using System.Collections.Generic;
using UnityEngine;

public class MinigameSliceObject : BaseMinigame
{
    [SerializeField] private GameObject _sliceablePrefab;
    [SerializeField] private int _objectsToSlice = 2;
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
        for (int i = 0; i < _objectsToSlice; i++)
        {
            SpawnSliceableObject();
        }

        // Create a runtime instance of the trail renderer but keep it disabled
        if (_trailEffect != null)
        {
            _activeTrail = Instantiate(_trailEffect);
            _activeTrail.gameObject.SetActive(false);
            _activeTrail.emitting = false;
            _activeTrail.Clear();
        }
    }

    private void SpawnSliceableObject()
    {
        GameObject sliceableObject = Instantiate(_sliceablePrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);

        // Choose a random required slice direction and rotate the object accordingly
        Vector2 requiredDirection = GetRandomDirection();
        float angle = Mathf.Atan2(requiredDirection.y, requiredDirection.x) * Mathf.Rad2Deg;
        sliceableObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Get the sliceable component so the required direction is preserved and accessible at runtime
        var so = sliceableObject.GetComponent<SliceableObject>();

        // Store the direction according to the object's rotated local X so it matches transform.right
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
                        if (_activeTrail != null) { _activeTrail.emitting = false; _activeTrail.Clear(); _activeTrail.gameObject.SetActive(false); }
                        _slicedThisDrag.Add(id);

                        _objectsToSlice--;

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

    private Vector2 GetRandomPositionInBounds()
    {
        Vector2 min = boundsCenter - boundsSize / 2;
        Vector2 max = boundsCenter + boundsSize / 2;

        return new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y)
        );
    }

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }
}