using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MinigameMath : BaseMinigame
{
    [Header("Prefabs & Layout")]
    [SerializeField] private GameObject _handPrefab;
    [SerializeField] private GameObject _mathTextPrefab;
    [SerializeField] private int _numberOfHands = 2;
    [SerializeField] private float _spacing = 1.0f; // Space between each hand horizontally
    
    [Header("Centering & Offsets")]
    [SerializeField] private float _textYOffset = 1.5f;
    [SerializeField] private float _handsYOffset = 0f;
    
    private int _currentTotal;
    private int _targetTotal;
    
    private GameObject _spawnedTextInstance;
    private TextMeshPro _mathDisplayText;
    private readonly List<CountableObject> _spawnedHands = new List<CountableObject>();

    protected override void StartMinigame()
    {
        _currentTotal = 0;

        // Target between 6 and max number of fingers.
        _targetTotal = UnityEngine.Random.Range(6, _numberOfHands * 5 + 1);

        ClearExistingObjects();
        
        // Spawn text first so reference is ready for UI updates
        SpawnTextPrefab();
        
        // Spawn hands and recalculate total
        SpawnCenteredHands();
    }

    private void SpawnTextPrefab()
    {
        if (_mathTextPrefab != null)
        {
            // Instantiate the text prefab as a child of this minigame
            _spawnedTextInstance = Instantiate(_mathTextPrefab, transform);
            
            // Get the text component reference immediately
            _mathDisplayText = _spawnedTextInstance.GetComponent<TextMeshPro>(); 

            if (_mathDisplayText != null)
            {
                // Force text alignment to center
                _mathDisplayText.alignment = TextAlignmentOptions.Center;
            }

            // Position the text centered horizontally above the hands
            if (_spawnedTextInstance.TryGetComponent<RectTransform>(out var textRect))
            {
                textRect.anchoredPosition = new Vector2(0f, _textYOffset);
            }
            else
            {
                _spawnedTextInstance.transform.localPosition = new Vector3(0f, _textYOffset, 0f);
            }
        }
    }

    private void SpawnCenteredHands()
    {
        float totalWidth = (_numberOfHands - 1) * _spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < _numberOfHands; i++)
        {
            float currentX = startX + (i * _spacing);
            
            // Instantiate as a child of this minigame first
            GameObject go = Instantiate(_handPrefab, transform);
            
            // Explicitly set local position and rotation relative to the parent
            go.transform.localPosition = new Vector3(currentX, _handsYOffset, 0f);
            go.transform.localRotation = Quaternion.identity;
            
            if (go.TryGetComponent<CountableObject>(out var countable))
            {
                countable.OnValueChanged += HandleHandValueChanged;
                _spawnedHands.Add(countable);
            }
        }
        
        RecalculateTotal();
    }

    private void HandleHandValueChanged(CountableObject hand)
    {
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        _currentTotal = 0;
        foreach (var hand in _spawnedHands)
        {
            if (hand != null)
            {
                _currentTotal += hand.CurrentValue;
            }
        }

        UpdateUI();
        CheckWinCondition();
    }

    private void UpdateUI()
    {
        if (_mathDisplayText != null)
        {
            // Builds a math equation display
            List<string> values = new List<string>();
            foreach (var hand in _spawnedHands)
            {
                values.Add(hand != null ? hand.CurrentValue.ToString() : "0");
            }

            _mathDisplayText.text = $"Goal: {_targetTotal}";
        }
    }

    private void CheckWinCondition()
    {
        if (_currentTotal == _targetTotal)
        {
            WinGame();
        }
    }

    private void ClearExistingObjects()
    {
        // Clean up text prefab instance
        if (_spawnedTextInstance != null)
        {
            Destroy(_spawnedTextInstance);
            _spawnedTextInstance = null;
            _mathDisplayText = null;
        }

        // Clean up hands
        foreach (var hand in _spawnedHands)
        {
            if (hand != null)
            {
                hand.OnValueChanged -= HandleHandValueChanged;
                Destroy(hand.gameObject);
            }
        }
        _spawnedHands.Clear();
    }

    protected override void UpdateMinigame() { }

    protected override void ApplyDifficultySettings()
    {
        switch(CurrentDifficulty)
        {
            case Difficulty.Easy:
                _numberOfHands = 2;
                break;
            case Difficulty.Normal:
                _numberOfHands = 3;
                break;
            case Difficulty.Hard:
                _numberOfHands = 4;
                break;
            case Difficulty.Boss:
                _numberOfHands = 5;
                break;
        }
    }
}