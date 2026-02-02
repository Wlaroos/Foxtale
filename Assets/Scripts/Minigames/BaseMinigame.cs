using UnityEngine;

public abstract class BaseMinigame : MonoBehaviour
{
    protected Vector2 boundsCenter;
    protected Vector2 boundsSize;
    protected float gameTimer;
    protected bool gameActive;

    public System.Action OnWin;
    public System.Action OnFail;

    [SerializeField] private string minigameText; // Serialized field for Unity Inspector

    private LayerMask _minigameLayerMask;

    public string MinigameText
    {
        get => minigameText;
        protected set => minigameText = value;
    }

    public LayerMask MinigameLayerMask
    {
        get => _minigameLayerMask;
        protected set => _minigameLayerMask = value;
    }

    public void Initialize(Vector2 center, Vector2 size, float timer)
    {
        boundsCenter = center;
        boundsSize = size;
        gameTimer = timer;
        gameActive = true;
        _minigameLayerMask = LayerMask.GetMask("Minigame");
        StartMinigame();
    }

    void Update()
    {
        if (gameActive)
        {
            gameTimer -= Time.deltaTime;

            if (gameTimer <= 0)
            {
                FailGame();
            }

            UpdateMinigame();
        }
    }

    protected void WinGame()
    {
        gameActive = false;
        OnWin?.Invoke();
        Destroy(gameObject);
    }

    protected void FailGame()
    {
        gameActive = false;
        OnFail?.Invoke();
        Destroy(gameObject);
    }

    protected abstract void StartMinigame();
    protected abstract void UpdateMinigame();

}