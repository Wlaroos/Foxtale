using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject _choicesPanel;
    [SerializeField] private GameObject[] _choices;
    private TextMeshProUGUI[] _choicesText;
    private Button[] _choicesButtons;

    [Header("Ink Story")]
    [SerializeField] private TextAsset _introDialogueJSONAsset;
    [SerializeField] public TextAsset _firstBigPipJSONAsset;
    [SerializeField] public TextAsset _secondBigPipJSONAsset;
    [SerializeField] public TextAsset _endingDialogueJSONAsset;
    private Story _currentStory;
    private bool _isDialoguePlaying;
    [SerializeField] private TextMeshProUGUI _lmbText;
    [SerializeField] private TextMeshProUGUI _rmbText;
    private const string FACE_TAG = "Face";
    private const string EXIT_GAME_TAG = "ExitGame";
    private bool _isWaitingForExternal = false;

    [Header("Typewriter Effect")]
    [SerializeField] private bool _enableTypewriterEffect = false;
    [SerializeField] private float _typingSpeed = 0.05f;

    [Header("Audio SFX")]
    [SerializeField] private AudioClip[] _babbleSounds;
    [Range(1, 5)] 
    [SerializeField] private int _babbleFrequency = 2; // Play sound every x characters
    private const float BABBLE_PITCH_VARIANCE = 0.1f;
    public float _mainPitch = 1f;
    [SerializeField] private bool _stopAudioOnFinish = true;
    private AudioSource _audioSource;

    private bool _isTyping;
    private string _fullText;
    private Coroutine _typingCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _audioSource = GetComponent<AudioSource>();
        _choicesText = new TextMeshProUGUI[_choices.Length];
        _choicesButtons = new Button[_choices.Length];

        for (int i = 0; i < _choices.Length; i++)
        {
            _choicesText[i] = _choices[i].GetComponentInChildren<TextMeshProUGUI>(true);
            _choicesButtons[i] = _choices[i].GetComponent<Button>();
        }
    }

    private void Start()
    {
        _dialoguePanel.SetActive(false);
        _currentStory = new Story(_introDialogueJSONAsset.text);

        _currentStory.BindExternalFunction("waitForCharacterSelect", () =>
        {
            StartCoroutine(WaitForCharacterSelection());
        });

        _currentStory.BindExternalFunction("waitForTutorial01", () =>
        {
            StartCoroutine(WaitForTutorial01());
        });

        MusicManager.Instance.PlayTalkingMusic();
    }

    private void Update()
    {
        if (_currentStory == null || !_isDialoguePlaying || _isWaitingForExternal) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (_isTyping)
            {
                FinishTypingEarly();
            }
            else if (_currentStory.currentChoices.Count == 0)
            {
                ContinueDialogue();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (_currentStory.canContinue)
            {
                SkipDialogue();
            }
        }
    }

    public void StartDialogue(Story story = null)
    {
        if (story != null)
        {
            _currentStory = story;
        }
        else if (_currentStory == null)
        {
            Debug.LogError("No story provided and no default story set.");
            return;
        }

        _isDialoguePlaying = true;
        _dialoguePanel.SetActive(true);
        _choicesPanel.SetActive(true);
        ContinueDialogue();
        MusicManager.Instance.PlayTalkingMusic();
    }

    public IEnumerator EndDialogue()
    {
        yield return new WaitForSeconds(0.2f);

        _isDialoguePlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueText.text = string.Empty;

        MinigameManager.Instance.StartRandomMinigame();
        MusicManager.Instance.PlayMinigameMusic();
    }

    private void ContinueDialogue()
    {
        if (_isWaitingForExternal) return;

        if (_currentStory.canContinue)
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }

            _fullText = _currentStory.Continue();

            HandleTags(_currentStory.currentTags);

            if (_enableTypewriterEffect)
            {
                _typingCoroutine = StartCoroutine(TypeText());
            }
            else
            {
                _dialogueText.text = _fullText;
            }

            DisplayChoices();
        }
        else
        {
            StartCoroutine(EndDialogue());
        }
    }

    private IEnumerator WaitForCharacterSelection()
    {
        _isWaitingForExternal = true;
        CharacterSelection.Instance._ready = true;

        while (!CharacterSelection.Instance._selected)
        {
            yield return null;
        }

        _isWaitingForExternal = false;

        if (_currentStory.canContinue)
        {
            ContinueDialogue();
        }
    }

    private IEnumerator WaitForTutorial01()
    {
        _isWaitingForExternal = true;

        MinigameManager.Instance.StartTutorial();

        while (!MinigameManager.Instance.TutorialFinished)
        {
            yield return null;
        }

        _isWaitingForExternal = false;

        if (_currentStory.canContinue)
        {
            ContinueDialogue();
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = _currentStory.currentChoices;

        if (currentChoices.Count > _choices.Length)
        {
            Debug.LogError("More choices than UI can support.");
        }

        for (int i = 0; i < _choices.Length; i++)
        {
            if (i < currentChoices.Count)
            {
                _choices[i].SetActive(true);
                _choicesText[i].text = currentChoices[i].text.Trim();

                _choicesButtons[i].onClick.RemoveAllListeners();

                int inkChoiceIndex = currentChoices[i].index;
                _choicesButtons[i].onClick.AddListener(() => MakeChoice(inkChoiceIndex));
            }
            else
            {
                _choices[i].SetActive(false);
            }
        }

        _lmbText.enabled = currentChoices.Count <= 0;
        _rmbText.enabled = _currentStory.canContinue;
    }

    public void MakeChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= _currentStory.currentChoices.Count)
        {
            return;
        }

        _currentStory.ChooseChoiceIndex(choiceIndex);

        foreach (GameObject choice in _choices)
        {
            choice.SetActive(false);
        }

        ContinueDialogue();
    }

    private void HandleTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag.Contains(":"))
            {
                string[] splitTag = tag.Split(':');
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                if (tagKey.Equals(FACE_TAG))
                {
                    FairyAnimation.Instance.ChangeFace(tagValue);
                }
            }
            else if (tag.Equals(EXIT_GAME_TAG))
            {
                ExitGame();
            }
        }
    }

    private void FinishTypingEarly()
    {
        if (_typingCoroutine != null)
        {
            StopCoroutine(_typingCoroutine);
        }
        _dialogueText.text = _fullText;
        _isTyping = false;
        _lmbText.color = new Color32(0, 0, 0, 200);

        if (_stopAudioOnFinish && _audioSource != null) _audioSource.Stop(); // Stop sound immediately

        ResizePanelToText();
        _typingCoroutine = null;
    }

    private IEnumerator TypeText()
    {
        _dialogueText.text = string.Empty;
        _isTyping = true;
        _lmbText.color = new Color32(0, 0, 0, 100);

        bool insideTag = false;
        int visibleCharacterCount = 0; // Track characters for babble frequency

        foreach (char letter in _fullText.ToCharArray())
        {
            if (letter == '<') insideTag = true;
            
            _dialogueText.text += letter;

            if (letter == '>') insideTag = false;

            if (!insideTag)
            {
                // Play sound every X characters, but not on spaces
                if (visibleCharacterCount % _babbleFrequency == 0 && !char.IsWhiteSpace(letter))
                {
                    PlayBabbleSound();
                }
                visibleCharacterCount++;

                ResizePanelToText();
                yield return new WaitForSeconds(_typingSpeed);
            }
        }

        _isTyping = false;
        _lmbText.color = new Color32(0, 0, 0, 200);
        ResizePanelToText();
        _typingCoroutine = null;
    }

    private void PlayBabbleSound()
    {
        if (_audioSource == null || _babbleSounds == null || _babbleSounds.Length == 0) return;

        // Randomize pitch for variety
        _audioSource.pitch = _mainPitch + Random.Range(-BABBLE_PITCH_VARIANCE, BABBLE_PITCH_VARIANCE);
        
        // Pick a random clip from the array
        int index = Random.Range(0, _babbleSounds.Length);
        _audioSource.PlayOneShot(_babbleSounds[index]);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        _dialoguePanel.SetActive(true);
        _currentStory.ChoosePathString("gameover");
        ContinueDialogue();

        yield return new WaitForSeconds(1f);
        CharacterSelection.Instance.Death3();
        yield return new WaitForSeconds(2f);
    }

    private void ExitGame()
    {
        Debug.Log("Exiting game...");
        
        StopAllCoroutines(); 
        
        _isDialoguePlaying = false;
        _isWaitingForExternal = false;
        _dialoguePanel.SetActive(false);

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
            //System.Diagnostics.Process.GetCurrentProcess().Kill();
        #endif
    }

    private void OnApplicationQuit()
    {
        // Stop all coroutines to prevent freezing
        StopAllCoroutines();

        // Additional cleanup logic if necessary
        _isDialoguePlaying = false;
        _isWaitingForExternal = false;
    }

    private void SkipDialogue()
    {
        if(!CharacterSelection.Instance._selected)
        {
            CharacterSelection.Instance.RandomlySelectCharacter();
        }

        MinigameManager.Instance.SkipTutorial();

        _currentStory.ChoosePathString("skipMain");

        _dialoguePanel.SetActive(false);
        _choicesPanel.SetActive(false);

        ContinueDialogue();
        ContinueDialogue();
    }

    private void ResizePanelToText()
    {
        // Force calculate to get the correct preferred height
        _dialogueText.ForceMeshUpdate();
        float preferredTextHeight = _dialogueText.preferredHeight;

        // Refs
        RectTransform panelRect = _dialoguePanel.GetComponent<RectTransform>();
        RectTransform textRect = _dialogueText.GetComponent<RectTransform>();

        // Offsets
        List<Choice> currentChoices = _currentStory.currentChoices;

        float paddingTop = 40f;
        float paddingBottom = 80f;

        if(currentChoices.Count > 0)
        {
            paddingTop = 40f;
            paddingBottom = 40f;
        }
        else
        {
            paddingTop = 40f;
            paddingBottom = 80f;
        }

        // Calculate panel height with text and padding
        float totalPanelHeight = preferredTextHeight + paddingTop + paddingBottom;

        // Set panel height
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, totalPanelHeight);

        // Text's position within the panel
        textRect.offsetMax = new Vector2(textRect.offsetMax.x, -paddingTop);
        textRect.offsetMin = new Vector2(textRect.offsetMin.x, paddingBottom);
    }

    public void StartBigPip1Dialogue()
    {
        StartDialogue(new Story(_firstBigPipJSONAsset.text));
    }

    public void StartBigPip2Dialogue()
    {
        StartDialogue(new Story(_secondBigPipJSONAsset.text));
    }

    public void StartEndingDialogue()
    {
        StartDialogue(new Story(_endingDialogueJSONAsset.text));
    }
}
