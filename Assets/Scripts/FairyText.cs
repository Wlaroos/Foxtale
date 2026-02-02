using System;
using UnityEngine;
using TMPro;

public class FairyText : MonoBehaviour
{
    [SerializeField] private string[] _fairyMessage = 
    {
        "Welcome, wandering soul, to the trial you must endure to regain your corporeal form",
        "I've been assigned as the test administrator for these little games.",
        "First, you must inhabit a vessel from one of these podiums to proceed.",
        "Can't really play games with flimsy ghost mitts, now can you?",
        "If you survive, that'll be your new body.",
        "Now,",
        "CHOOSE <br> A <br> VESSEL",
        "And just to up the stakes, I've implanted an innocent soul inside each of these.",
        "You get three strikes, each resulting in the loss of one vessel and the soul within.",
        "However, each success will reward you with some currency to spend later on.",
        "If you collect enough, I might be persuaded to let you keep your soul and body.",
        "Let's begin, shall we?"
    };
    [SerializeField] private TextMeshProUGUI _lmbText;
    private TypewriterEffect _typewriterRef;
    private int _currentMessageIndex = 0;
    private bool _fairyFinished = false;

    private void Awake()
    {
        _typewriterRef = GetComponent<TypewriterEffect>();

        if (_typewriterRef == null)
        {
            Debug.LogError("TypewriterEffect component is missing.");
        }
        else
        {
            _typewriterRef.OnTypingStatusChanged += HandleTypingStatusChanged; // Subscribe to the event
        }
    }

    private void OnDestroy()
    {
        if (_typewriterRef != null)
        {
            _typewriterRef.OnTypingStatusChanged -= HandleTypingStatusChanged; // Unsubscribe to avoid memory leaks
        }
    }

    private void Start()
    {
        NextMessage();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextMessage();
        }

        if (Input.GetMouseButtonDown(1))
        {
            _currentMessageIndex = -1;
            NextMessage();
        }
    }

    private void NextMessage()
    {
        if (_currentMessageIndex == -1 && !_fairyFinished)
        {
            _typewriterRef.SetFullText("");
            _fairyFinished = true;
            MinigameManager.Instance.StartRandomMinigame();
            return;
        }

        if (_typewriterRef != null && !_typewriterRef.IsTyping && !_fairyFinished)
        {
            switch (_currentMessageIndex)
            {
                case 0:
                    FairyAnimation.Instance.ChangeFace("Smile");
                    break;
                case 1:
                    break;
                case 2: 
                    break;
                case 3:
                    FairyAnimation.Instance.ChangeFace("Grin");
                    break;
                case 4:
                    FairyAnimation.Instance.ChangeFace("Smile");
                    break;
                case 5:
                    FairyAnimation.Instance.ChangeFace("Stare");
                    break;
                case 6:
                    if (!CharacterSelection.Instance._selected)
                    {
                        CharacterSelection.Instance._ready = true;
                        return; // Wait until character is selected
                    }
                    FairyAnimation.Instance.ChangeFace("Evil");
                    break;
                case 7:
                    FairyAnimation.Instance.ChangeFace("Evil");
                    break;
                case 8:
                    FairyAnimation.Instance.ChangeFace("Smile");
                    break;
                case 9:
                    break;
                case 10:
                    FairyAnimation.Instance.ChangeFace("Grin");
                    break;
                case 11:
                    FairyAnimation.Instance.ChangeFace("Smile");
                    break;

                case int index when index >= _fairyMessage.Length: // End of messages
                    _fairyFinished = true;
                    MinigameManager.Instance.StartRandomMinigame();
                    return;
            }
            if (_currentMessageIndex < _fairyMessage.Length && !_fairyFinished)
            {
                _typewriterRef.SetFullText(_fairyMessage[_currentMessageIndex]);
            }
            _currentMessageIndex++;
        }
    }

    private void HandleTypingStatusChanged(bool isTyping)
    {
        if (isTyping)
        {
            _lmbText.color = new Color32(0, 0, 0, 130);
        }
        else
        {
            _lmbText.color = new Color32(0, 0, 0, 255);
        }
    }
}
