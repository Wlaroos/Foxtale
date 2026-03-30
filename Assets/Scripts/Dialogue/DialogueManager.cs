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
    [SerializeField] private TextAsset _inkJSONAsset;
    private Story _currentStory;
    private bool _isDialoguePlaying;
    [SerializeField] private TextMeshProUGUI _lmbText;
    private const string FACE_TAG = "Face";
    private const string EXIT_GAME_TAG = "ExitGame";
    private bool _isWaitingForExternal = false;

    [Header("Typewriter Effect")]
    [SerializeField] private bool _enableTypewriterEffect = false;
    [SerializeField] private float _typingSpeed = 0.05f;

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
        _currentStory = new Story(_inkJSONAsset.text);

        _currentStory.BindExternalFunction("waitForCharacterSelect", () =>
        {
            StartCoroutine(WaitForCharacterSelection());
        });

        _currentStory.BindExternalFunction("waitForTutorial01", () =>
        {
            StartCoroutine(WaitForTutorial01());
        });

        StartDialogue(_currentStory);
    }

private void Update()
{
    if (_currentStory == null ||!_isDialoguePlaying || _isWaitingForExternal) return;

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
    else if(Input.GetMouseButtonDown(1))
    {
        if(_currentStory.canContinue)
        {
            SkipDialogue();
        }
    }
}

    public void StartDialogue(Story story)
    {
        _currentStory = story;
        _isDialoguePlaying = true;
        _dialoguePanel.SetActive(true);
        ContinueDialogue();
    }

    public IEnumerator EndDialogue()
    {
        yield return new WaitForSeconds(0.2f);

        _isDialoguePlaying = false;
        _dialoguePanel.SetActive(false);
        _dialogueText.text = string.Empty;

        MinigameManager.Instance.StartRandomMinigame();
        //Debug.Log("Dialogue ended, starting minigame...");
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
            yield return null; // Wait until the character is selected
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
            yield return null; // Wait until the tutorial is completed
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
            
            // We use the choice's internal index from Ink, not the loop index
            int inkChoiceIndex = currentChoices[i].index; 
            _choicesButtons[i].onClick.AddListener(() => MakeChoice(inkChoiceIndex));
        }
        else
        {
            _choices[i].SetActive(false);
        }
    }
}

public void MakeChoice(int choiceIndex)
{
    // Check if the choice actually exists in the current state
    if (choiceIndex < 0 || choiceIndex >= _currentStory.currentChoices.Count)
    {
        //Debug.LogWarning("Selected choice index is no longer valid. Ignoring click.");
        return;
    }

    _currentStory.ChooseChoiceIndex(choiceIndex);
    
    // Deactivate choices immediately
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
        _lmbText.color = new Color32(0, 0, 0, 255);
        _typingCoroutine = null;
    }

    private IEnumerator TypeText()
    {
        _dialogueText.text = string.Empty;
        _isTyping = true;
        _lmbText.color = new Color32(0, 0, 0, 130);

        bool insideTag = false;

        // We use a simple loop. The 'Update' method now catches the click 
        // and calls FinishTypingEarly(), which stops this coroutine.
        foreach (char letter in _fullText.ToCharArray())
        {
            if (letter == '<') insideTag = true;
            if (letter == '>') insideTag = false;

            if (!insideTag)
            {
                _dialogueText.text += letter;
                yield return new WaitForSeconds(_typingSpeed);
            }
            else
            {
                _dialogueText.text += letter;
            }
        }

        _isTyping = false;
        _lmbText.color = new Color32(0, 0, 0, 255);
        _typingCoroutine = null;
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

        // Cleanup Ink
        if (_currentStory != null)
        {
            _currentStory.UnbindExternalFunction("waitForCharacterSelect");
        }

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

        _currentStory.ChoosePathString("skipMain");

        _dialoguePanel.SetActive(false);
        _choicesPanel.SetActive(false);

        ContinueDialogue();
        ContinueDialogue();
    }
}
