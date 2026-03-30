using UnityEngine;

public abstract class BaseMinigame : MonoBehaviour
{
    protected Vector2 _boundsCenter;
    protected Vector2 _boundsSize;
    protected float _gameTimer;
    protected bool _gameActive;

    public System.Action OnWin;
    public System.Action OnFail;

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

    public void Initialize(Vector2 center, Vector2 size, float timer)
    {
        _boundsCenter = center;
        _boundsSize = size;
        _gameTimer = timer;
        _gameActive = true;
        _minigameLayerMask = LayerMask.GetMask("Minigame");
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

    protected Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }
}