using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance;

    [Header("Minigame Settings")]
    [SerializeField] private float _gameTimer = 5f;
    [SerializeField] private float _bigMinigameTimer = 10f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _timeBetweenMinigames = 1f;
    [Space(10)]
    [SerializeField] private int[] _numberOfMinigamesBeforeBigMinigame = { 5, 7, 7 };

    [Header("UI References")]
    [SerializeField] private Slider _timerSlider;
    [SerializeField] private TextMeshProUGUI _minigameText;
    [SerializeField] private CoinManager _coinParticles;
    [SerializeField] private TextMeshProUGUI _moneyText;

    [Header("Pip UI Settings")]
    [SerializeField] private MinigamePip _pipPrefab;
    [SerializeField] private Transform _pipHolder;
    // If false, the player can just ignore failed attempts and keep playing until they win
    [SerializeField] private bool _markFailsOnPips = true;
    [Header("Minigames")]
    [SerializeField] private BaseMinigame[] _minigamePrefabs;
    [SerializeField] private BaseMinigame[] _bigMinigamePrefabs;
    [SerializeField] private Vector2 _boundsCenter = Vector2.zero;
    [SerializeField] private Vector2 _boundsSize = new Vector2(7.5f, 7.5f);

    [Header("Forced Minigame (Optional)")]
    [SerializeField] private BaseMinigame _forcedMinigame;

    [Header("Tutorial")]
    [SerializeField] private BaseMinigame[] _tutorialMinigames;

    // Internal State
    private BaseMinigame _currentMinigame;
    private SpriteRenderer _sr;
    private int _wins = 0;
    private int _fails = 0;
    private float _currentTimer;
    private int _money = 0;
    public int Money => _money;

    // Progress tracking
    private int _minigamesPlayed = 0; 
    private const float TIMER_DECREASE_AMOUNT = 0.5f;
    private const float MIN_TIMER_LIMIT = 2f;
    
    // Tutorial tracking
    private bool _tutorialFinished = false;
    public bool TutorialFinished => _tutorialFinished;
    private int _tutorialIndex = 0;

    private List<MinigamePip> _spawnedPips = new List<MinigamePip>();
    private int _bigPipsCompletedCount = 0;

    private float _selectedTimerDuration;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _moneyText.text = _money.ToString();
        
        // Initialize the UI pips at the start of the session
        SetupPipDisplay();
    }

    void Update()
    {
        if (_currentMinigame != null && _currentTimer > 0)
        {
            _currentTimer -= Time.deltaTime;
            _timerSlider.value = _currentTimer / _selectedTimerDuration;
        }
    }
    

    public void StartRandomMinigame()
    {
        StartCoroutine(StartMinigameWithDelay());
    }

    IEnumerator StartMinigameWithDelay(float timerOverride = 0f)
    {
        _minigameText.text = "Get ready...";
        yield return new WaitForSeconds(_timeBetweenMinigames);

        if (_currentMinigame != null) Destroy(_currentMinigame.gameObject);

        _selectedTimerDuration = _gameTimer; // Default to normal timer
        
        bool isBigMinigame = false;

        if(_minigamesPlayed >= _spawnedPips.Count)
        {
            yield break;
        }

        if(_spawnedPips[_minigamesPlayed].CurrentShape == MinigamePip.PipShape.Big) isBigMinigame = true;

        // Selection Logic
        // Forced Minigame
        if (_forcedMinigame != null)
        {
            _currentMinigame = Instantiate(_forcedMinigame, transform);
            MusicManager.Instance.PlayMinigameMusic();
        }
        // Tutorial Minigame
        else if (!_tutorialFinished && _tutorialIndex < _tutorialMinigames.Length)
        {
            _currentMinigame = Instantiate(_tutorialMinigames[_tutorialIndex], transform);
            MusicManager.Instance.PlayMinigameMusic();
        }
        // Check if the current pip is a "Big" minigame pip
        else if (_minigamesPlayed < _spawnedPips.Count && _spawnedPips[_minigamesPlayed].CurrentShape == MinigamePip.PipShape.Big)
        {
            // Pick a random Big Minigame
            int randomIndex = Random.Range(0, _bigMinigamePrefabs.Length);
            _currentMinigame = Instantiate(_bigMinigamePrefabs[randomIndex], transform);
            MusicManager.Instance.PlayBossMusic();
        }
        // Normal Minigames
        else
        {
            int randomIndex = Random.Range(0, _minigamePrefabs.Length);
            _currentMinigame = Instantiate(_minigamePrefabs[randomIndex], transform);
            MusicManager.Instance.PlayMinigameMusic();
        }

        // Logic for deciding the duration
        if (timerOverride > 0f) 
            _selectedTimerDuration = timerOverride; // Use manual override
        else if (isBigMinigame) 
            _selectedTimerDuration = _bigMinigameTimer; // Use the Big Minigame override
        else 
            _selectedTimerDuration = _gameTimer; // Use standard game timer

        // Initialization
        BaseMinigame.Difficulty targetDifficulty;

        if (isBigMinigame)
        {
            targetDifficulty = BaseMinigame.Difficulty.Boss;
        }
        else
        {
            targetDifficulty = GetDifficultyLevel();
        }

        _currentMinigame.Initialize(_boundsCenter, _boundsSize, _selectedTimerDuration, targetDifficulty);

        _currentMinigame.OnWin = HandleWin;
        _currentMinigame.OnFail = HandleFail;

        _minigameText.text = _currentMinigame.MinigameText;
        _currentTimer = _selectedTimerDuration;
        _timerSlider.value = 1f;

        // Progress Pips
        UpdatePipStates();
    }

    void HandleWin()
    {
        if (_tutorialFinished == false && _tutorialIndex < _tutorialMinigames.Length)
        {
            _tutorialIndex++;
            _tutorialFinished = true;
            _minigameText.text = "";
            StartCoroutine(ColorToFade(Color.green, 0.75f));
        }
        else
        {
            _minigamesPlayed++;
            _wins++;

            if (_minigamesPlayed - 1 < _spawnedPips.Count)
            {
                _spawnedPips[_minigamesPlayed - 1].SetWin();
            }

            // Logic for visuals/rewards
            string[] faces = { "Stare", "Angry", "Confused", "Sad", "Squint", "Cat", "Creeper", "Deadpan", "Surprised", "SuperStare", "Fresh", "Shock" };
            FairyAnimation.Instance.ChangeFace(faces[Random.Range(0, faces.Length)]);
            _coinParticles.CreateCoins(10, 0.05f);
            _minigameText.text = "";
            StartCoroutine(ColorToFade(Color.green, 0.75f));

            bool isDialogueActive = CheckBigMinigameProgression();

            if (!isDialogueActive) 
            {
                StartRandomMinigame();
            }
        }

        SFXManager.Instance.PlayMinigameWin();
    }

    void HandleFail()
    {
        _fails++;
        _lives--;

        FairyAnimation.Instance.ChangeFace("Evil");
        _minigameText.text = "";
        StartCoroutine(ColorToFade(Color.red, 0.75f));

        ScreenShake.ShakeOnce(1, 5);

        SFXManager.Instance.PlayMinigameLose();

        if(_tutorialFinished == false && _tutorialIndex < _tutorialMinigames.Length)
        {
            _minigameText.text = "Game Over!";
            _tutorialFinished = true;
            DialogueManager.Instance.StartGameOverDialogue("tutorial");
            return;
        }

        if (_markFailsOnPips)
        {
            if (_minigamesPlayed < _spawnedPips.Count)
                _spawnedPips[_minigamesPlayed].SetFail();
            
            _minigamesPlayed++;

            // Check if the pip we just failed was a Big one
            bool isDialogueActive = CheckBigMinigameProgression();
            
            if (_lives > 0 && !isDialogueActive)
            {
                StartRandomMinigame();
            }
        }

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
            DialogueManager.Instance.StartGameOverDialogue("gameover");
        }
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

        SFXManager.Instance.PlayCoinCollect();
    }

    public void StartTutorial()
    {
        _tutorialFinished = false;
        StartCoroutine(StartMinigameWithDelay(60f));
    }

    public void SkipTutorial()
    {
        _tutorialFinished = true;
        _tutorialIndex = _tutorialMinigames.Length;
    }

    private void SetupPipDisplay()
    {
        // Clear existing
        foreach (var p in _spawnedPips) Destroy(p.gameObject);
        _spawnedPips.Clear();


        // First, add the very bottom pip
        _spawnedPips.Add(CreatePip(MinigamePip.PipShape.Bottom));

        for (int i = 0; i < _numberOfMinigamesBeforeBigMinigame.Length; i++)
        {
            for (int j = 0; j < _numberOfMinigamesBeforeBigMinigame[i]; j++)
            {
                // If it's the very first index, we already made the Bottom Pip
                if (i == 0 && j == 0) continue; 
                _spawnedPips.Add(CreatePip(MinigamePip.PipShape.Middle));
            }
            // Add the Big minigame at the end of the segment
            _spawnedPips.Add(CreatePip(MinigamePip.PipShape.Big));
        }
    }

    private MinigamePip CreatePip(MinigamePip.PipShape shape)
    {
        MinigamePip newPip = Instantiate(_pipPrefab, _pipHolder);
        newPip.Initialize(shape);
        
        // This makes the newest instantiated pip appear at the TOP of the list
        newPip.transform.SetAsFirstSibling(); 
        return newPip;
    }

    private void UpdatePipStates()
    {
        for (int i = 0; i < _spawnedPips.Count; i++)
        {
            // Only the pip matching current game count flashes, will flash yellow if tutorial
            _spawnedPips[i].SetActive(i == _minigamesPlayed, _tutorialIndex < _tutorialMinigames.Length);
        }
    }

    private bool CheckBigMinigameProgression()
    {
        int completedIndex = _minigamesPlayed - 1;

        if (completedIndex >= 0 && completedIndex < _spawnedPips.Count)
        {
            if (_spawnedPips[completedIndex].CurrentShape == MinigamePip.PipShape.Big)
            {
                switch (_bigPipsCompletedCount)
                {
                    case 0:
                        DialogueManager.Instance.StartBigPip1Dialogue();
                        break;
                    case 1:
                        DialogueManager.Instance.StartBigPip2Dialogue();
                        break;
                    case 2:
                        DialogueManager.Instance.StartEndingDialogue();
                        break;
                }

                _bigPipsCompletedCount++;
                return true;
            }
        }
        return false;
    }

    public void ResumeMinigames()
    {
        StartRandomMinigame();
    }

    private BaseMinigame.Difficulty GetDifficultyLevel()
{
    // If we have completed 0 Big Pips, we are in the first segment (Easy)
    // If 1, we are in the second segment (Medium)
    // If 2+, we are in the final segment (Hard)
    switch (_bigPipsCompletedCount)
    {
        case 0: return BaseMinigame.Difficulty.Easy;
        case 1: return BaseMinigame.Difficulty.Normal;
        default: return BaseMinigame.Difficulty.Hard;
    }
}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_boundsCenter, _boundsSize);
    }
}