using UnityEngine;

public class MinigameMovingTarget : BaseMinigame
{
    [SerializeField] private GameObject _movingTargetPrefab;
    [SerializeField] private float _obstacleSpeed = 8f;
    [SerializeField] private GameObject _soulParticlePrefab;
    [SerializeField] private AnimationCurve _speedAnimationCurve;
    [SerializeField] private int _numberOfObjects = 1;
    private GameObject[] _movingTargets;
    private Vector2[] _currentDirections;
    private float _totalDuration; // Store the initial time to calculate progress
    private Camera _cachedCam;

    protected override void StartMinigame()
    {
        _cachedCam = Camera.main;
        _totalDuration = _gameTimer;

        _movingTargets = new GameObject[_numberOfObjects];
        _currentDirections = new Vector2[_numberOfObjects];

        for (int i = 0; i < _numberOfObjects; i++)
        {
            _movingTargets[i] = Instantiate(_movingTargetPrefab, GetRandomPositionInBounds(), Quaternion.identity, transform);
            _currentDirections[i] = GetRandomDirection();
        }
    }

    protected override void UpdateMinigame()
    {
        if (_movingTargets == null) return;

        float progress = Mathf.Clamp01((_totalDuration - _gameTimer) / _totalDuration);
        float speedMultiplier = _speedAnimationCurve.Evaluate(progress);
        Vector2 halfSize = _boundsSize / 2f;

        for (int i = 0; i < _movingTargets.Length; i++)
        {
            if (_movingTargets[i] == null) continue;

            // Move the target in the current direction
            _movingTargets[i].transform.position += (Vector3)(_currentDirections[i] * Time.deltaTime * _obstacleSpeed * speedMultiplier);

            // Check if the target hits the bounds and bounce
            Vector2 position = _movingTargets[i].transform.position;

            if (position.x <= _boundsCenter.x - halfSize.x || position.x >= _boundsCenter.x + halfSize.x)
            {
                _currentDirections[i].x = -_currentDirections[i].x;
                position.x = Mathf.Clamp(position.x, _boundsCenter.x - halfSize.x, _boundsCenter.x + halfSize.x);
            }
            if (position.y <= _boundsCenter.y - halfSize.y || position.y >= _boundsCenter.y + halfSize.y)
            {
                _currentDirections[i].y = -_currentDirections[i].y;
                position.y = Mathf.Clamp(position.y, _boundsCenter.y - halfSize.y, _boundsCenter.y + halfSize.y);
            }

            _movingTargets[i].transform.position = position;
        }

        // Check for mouse click on any target
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = _cachedCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, MinigameLayerMask);

            if (hit.collider != null)
            {
                for (int i = 0; i < _movingTargets.Length; i++)
                {
                    if (_movingTargets[i] != null && hit.collider.gameObject == _movingTargets[i])
                    {
                        if (_soulParticlePrefab != null)
                        {
                            Instantiate(_soulParticlePrefab, _movingTargets[i].transform.position, Quaternion.identity);
                        }

                        SFXManager.Instance.PlayBoneBreak();
                        
                        Destroy(_movingTargets[i]);
                        _movingTargets[i] = null;

                        // Win if all targets are destroyed
                        bool allDestroyed = true;
                        for (int j = 0; j < _movingTargets.Length; j++)
                        {
                            if (_movingTargets[j] != null)
                            {
                                allDestroyed = false;
                                break;
                            }
                        }
                        if (allDestroyed)
                        {
                            WinGame();
                        }
                        break;
                    }
                }
            }
        }
    }
}