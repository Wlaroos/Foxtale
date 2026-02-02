using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;
    [SerializeField] private float _gameTimer = 5f;
    [SerializeField] private float _timeBetweenMinigames = 1f;
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private TextMeshProUGUI _minigameText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private BaseMinigame[] _minigamePrefabs;
    [SerializeField] private Vector2 _boundsCenter = Vector2.zero;
    [SerializeField] private Vector2 _boundsSize = new Vector2(7.5f, 7.5f);
    private BaseMinigame _currentMinigame; // Reference to the currently active minigame
    private SpriteRenderer _sr;
    private int _wins = 0;
    private int _fails = 0;
    private float _currentTimer;
    private int _money = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _moneyText.text = _money.ToString();
    }

    void Update()
    {
        // Update the timer slider while the minigame is running
        if (_currentMinigame != null && _currentTimer > 0)
        {
            _currentTimer -= Time.deltaTime;
            _timerSlider.value = _currentTimer / _gameTimer;

            if (_currentTimer <= 0)
            {
                HandleFail(); // Trigger fail if the timer runs out
            }
        }
    }

    public void StartRandomMinigame()
    {
        StartCoroutine(StartMinigameWithDelay());
    }

    IEnumerator StartMinigameWithDelay()
    {
        // Wait for the specified time between minigames
        _minigameText.text = "Get ready for the next minigame...";
        yield return new WaitForSeconds(_timeBetweenMinigames);

        // Clean up the previous minigame if it exists
        if (_currentMinigame != null)
        {
            Destroy(_currentMinigame.gameObject);
        }

        // Select a random minigame prefab
        int randomIndex = Random.Range(0, _minigamePrefabs.Length);
        _currentMinigame = Instantiate(_minigamePrefabs[randomIndex], transform);

        // Initialize the new minigame
        _currentMinigame.Initialize(_boundsCenter, _boundsSize, _gameTimer);
        _currentMinigame.OnWin = HandleWin;
        _currentMinigame.OnFail = HandleFail;

        // Update the minigame text
        _minigameText.text = _currentMinigame.MinigameText;

        // Reset the timer and update the UI
        _currentTimer = _gameTimer;
        _timerSlider.value = 1f;
    }

    void HandleWin()
    {
        _wins++;
        _money += 10; // Increment money after each win
        _moneyText.text = _money.ToString();
        _minigameText.text = "You won!";
        StartCoroutine(ColorToFade(Color.green, 0.75f));
        StartRandomMinigame();
    }

    void HandleFail()
    {
        _fails++;
        _minigameText.text = "You failed!";
        StartCoroutine(ColorToFade(Color.red, 0.75f));
        ScreenShake.ShakeOnce(1, 5);
        StartRandomMinigame();
    }

    private IEnumerator ColorToFade(Color color, float duration)
    {
        Color originalColor = color;
        Color targetColor = Color.white;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _sr.color = Color.Lerp(originalColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _sr.color = targetColor;
    }

        private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set the color for the bounds
        Gizmos.DrawWireCube(_boundsCenter, _boundsSize); // Draw the bounds as a wireframe cube
    }
}