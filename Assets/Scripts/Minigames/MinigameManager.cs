using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private float _gameTimer = 5f;
    [SerializeField] private float _timeBetweenMinigames = 1f;
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private TextMeshProUGUI _minigameText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private BaseMinigame[] _minigamePrefabs;
    private BoxCollider2D _bounds;
    private Vector2 _boundsPadding = new Vector2(0.5f, 0.5f);
    private BaseMinigame _currentMinigame; // Reference to the currently active minigame
    private int _wins = 0;
    private int _fails = 0;
    private float _currentTimer;
    private int _money = 0;

    void Start()
    {
        _bounds = GetComponent<BoxCollider2D>();

        // Apply padding to the bounds
        if (_bounds != null)
        {
            _bounds.size -= _boundsPadding;
        }

        _moneyText.text = _money.ToString();
        StartRandomMinigame();
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

    void StartRandomMinigame()
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
        _currentMinigame.Initialize(_bounds, _gameTimer);
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
        StartRandomMinigame();
    }

    void HandleFail()
    {
        _fails++;
        _minigameText.text = "You failed!";
        StartRandomMinigame();
    }
}