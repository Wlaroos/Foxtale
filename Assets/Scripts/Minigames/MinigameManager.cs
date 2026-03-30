using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;
    [SerializeField] private float _gameTimer = 5f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _timeBetweenMinigames = 1f;
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private TextMeshProUGUI _minigameText;
    [SerializeField] CoinManager _coinParticles;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private BaseMinigame[] _minigamePrefabs;
    [SerializeField] private Vector2 _boundsCenter = Vector2.zero;
    [SerializeField] private Vector2 _boundsSize = new Vector2(7.5f, 7.5f);
    [SerializeField] private BaseMinigame _forcedMinigame;
    [SerializeField] private BaseMinigame[] _tutorialMinigames;
    private BaseMinigame _currentMinigame; // Reference to the currently active minigame
    private SpriteRenderer _sr;
    private int _wins = 0;
    private int _fails = 0;
    private float _currentTimer;
    private int _money = 0;
    public int Money => _money;
    private int _minigamesPlayed = 0; // Counter for the number of minigames played
    private const float _timerDecreaseAmount = 0.5f; // Amount to decrease the timer
    private const float _minTimerLimit = 2f; // Minimum timer limit
    private bool _tutorialFinished = false;
    public bool TutorialFinished => _tutorialFinished;
    private int _tutorialIndex = 0;

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
        }
    }

    public void StartRandomMinigame()
    {
        StartCoroutine(StartMinigameWithDelay());
    }

    IEnumerator StartMinigameWithDelay(float timerOverride = 0f)
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

        if (_forcedMinigame != null)
        {
            _currentMinigame = Instantiate(_forcedMinigame, transform);
        }
        else if (!_tutorialFinished && _tutorialIndex < _tutorialMinigames.Length)
        {
            _currentMinigame = Instantiate(_tutorialMinigames[_tutorialIndex], transform);
        }
        else
        {
            int randomIndex = Random.Range(0, _minigamePrefabs.Length);
            _currentMinigame = Instantiate(_minigamePrefabs[randomIndex], transform);
        }

        // Initialize the new minigame
        if(timerOverride > 0f)
        {
            _currentMinigame.Initialize(_boundsCenter, _boundsSize, timerOverride);
        }
        else
        {
            _currentMinigame.Initialize(_boundsCenter, _boundsSize, _gameTimer);
        }

        _currentMinigame.OnWin = HandleWin;
        _currentMinigame.OnFail = HandleFail;

        // Update the minigame text
        _minigameText.text = _currentMinigame.MinigameText;

        // Reset the timer and update the UI
        if(timerOverride > 0f)
        {
            _currentTimer = timerOverride;
        }
        else
        {
            _currentTimer = _gameTimer;
        }

        _timerSlider.value = 1f;

        // Increment the minigames played counter
        _minigamesPlayed++;

        // Decrease the timer every 5 minigames, but ensure it doesn't go below the minimum limit
        if (_minigamesPlayed % 5 == 0 && _gameTimer > _minTimerLimit)
        {
            _gameTimer = Mathf.Max(_gameTimer - _timerDecreaseAmount, _minTimerLimit);
        }
    }

    void HandleWin()
    {
        if(_tutorialFinished == false)
        {
            _tutorialIndex++;
            _tutorialFinished = true;
            StartCoroutine(ColorToFade(Color.green, 0.75f));
        }
        else
        {
            _wins++;

            string[] faces = { "Stare", "Angry", "Confused", "Sad", "Squint", "Cat" };
            FairyAnimation.Instance.ChangeFace(faces[Random.Range(0, faces.Length)]);

            _coinParticles.CreateCoins(10, 0.05f);

            _minigameText.text = "You won!";

            StartCoroutine(ColorToFade(Color.green, 0.75f));
            StartRandomMinigame();
        }
    }

    void HandleFail()
    {
        _fails++;
        _lives--;

        FairyAnimation.Instance.ChangeFace("Evil");

        _minigameText.text = "You failed!";

        StartCoroutine(ColorToFade(Color.red, 0.75f));

        if (_fails == 1)
        {
            CharacterSelection.Instance.Death1();
            StartRandomMinigame();
        }
        else if (_fails == 2)
        {
            CharacterSelection.Instance.Death2();
            FairyAnimation.Instance.ArmDefault();
            StartRandomMinigame();
        }
        else if (_lives <= 0)
        {
            _minigameText.text = "Game Over!";
            DialogueManager.Instance.GameOver();
        }

        ScreenShake.ShakeOnce(1, 5);
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

    public void AddMoney(int amount)
    {
        _money += amount;
        _moneyText.text = _money.ToString();
    }

    public void StartTutorial()
    {
        _tutorialFinished = false;
        StartCoroutine(StartMinigameWithDelay(500f));
    }

    public void SkipTutorial()
    {
        _tutorialFinished = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set the color for the bounds
        Gizmos.DrawWireCube(_boundsCenter, _boundsSize); // Draw the bounds as a wireframe cube
    }
}