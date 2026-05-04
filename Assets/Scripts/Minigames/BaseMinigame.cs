using UnityEngine;
using System.Collections.Generic;

public abstract class BaseMinigame : MonoBehaviour
{
    public enum Difficulty { Easy, Normal, Hard, Boss }
    protected Vector2 _boundsCenter;
    protected Vector2 _boundsSize;
    protected float _gameTimer;
    protected bool _gameActive;

    public System.Action OnWin;
    public System.Action OnFail;

    protected Difficulty CurrentDifficulty { get; private set; }

    protected bool IsBigMinigame { get; private set; }

    [SerializeField] private string _minigameText; // Serialized field for Unity Inspector

    private LayerMask _minigameLayerMask;

    public string MinigameText
    {
        get => _minigameText;
        protected set => _minigameText = value;
    }

    public LayerMask MinigameLayerMask
    {
        get => _minigameLayerMask;
        protected set => _minigameLayerMask = value;
    }

    public void Initialize(Vector2 center, Vector2 size, float timer, Difficulty difficulty = Difficulty.Normal)
    {
        _boundsCenter = center;
        _boundsSize = size;
        _gameTimer = timer;
        _gameActive = true;
        _minigameLayerMask = LayerMask.GetMask("Minigame");

        CurrentDifficulty = difficulty;
        ApplyDifficultySettings();

        StartMinigame();
    }

    void Update()
    {
        if (_gameActive)
        {
            _gameTimer -= Time.deltaTime;

            if (_gameTimer <= 0)
            {
                FailGame();
            }

            UpdateMinigame();
        }
    }

    protected void WinGame()
    {
        _gameActive = false;
        OnWin?.Invoke();
        Destroy(gameObject);
    }

    protected void FailGame()
    {
        _gameActive = false;
        OnFail?.Invoke();
        Destroy(gameObject);
    }

    protected abstract void StartMinigame();
    protected abstract void UpdateMinigame();

    protected Vector2 GetRandomPositionInBounds()
    {
        Vector2 min = _boundsCenter - _boundsSize / 2;
        Vector2 max = _boundsCenter + _boundsSize / 2;
        return new Vector2(Random.Range(min.x, max.x),Random.Range(min.y, max.y));
    }

    protected List<Vector2> GetRandomPositionsInBounds(int count, float minDistance, int maxAttempts = 250)
    {
        List<Vector2> positions = new List<Vector2>();
        
        Vector2 min = _boundsCenter - _boundsSize / 2f;
        Vector2 max = _boundsCenter + _boundsSize / 2f;

        for (int i = 0; i < count; i++)
        {
            bool foundValidPoint = false;

            // Try multiple times to find a spot that doesn't overlap
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Vector2 candidate = new Vector2(
                    Random.Range(min.x, max.x),
                    Random.Range(min.y, max.y)
                );

                if (IsPositionValid(candidate, positions, minDistance))
                {
                    positions.Add(candidate);
                    foundValidPoint = true;
                    break;
                }
            }

            if (!foundValidPoint)
            {
                Debug.LogWarning($"Could only find space for {positions.Count} points before giving up.");
                break;
            }
        }

        return positions;
    }

    private bool IsPositionValid(Vector2 candidate, List<Vector2> existingPositions, float minDistance)
    {
        foreach (Vector2 pos in existingPositions)
        {
            // Vector2.Distance is cleaner, but sqrMagnitude is faster for performance
            if (Vector2.SqrMagnitude(candidate - pos) < minDistance * minDistance)
            {
                return false;
            }
        }
        return true;
    }

    protected Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    // Child classes override this to change their internal values
    protected virtual void ApplyDifficultySettings()
    {
        // Default behavior: logic based on CurrentDifficulty
        switch (CurrentDifficulty)
        {
            case Difficulty.Easy:
                break;
            case Difficulty.Normal:
                break;
            case Difficulty.Hard:
                break;
            case Difficulty.Boss:
                break;
        }
    }
}