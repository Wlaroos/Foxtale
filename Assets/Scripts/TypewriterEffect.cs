using System;
using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float _typingSpeed = 0.05f;
    private TMP_Text _textComponent;
    private string _fullText;

    private bool _isTyping;
    public bool IsTyping
    {
        get => _isTyping;
        private set
        {
            if (_isTyping != value)
            {
                _isTyping = value;
                OnTypingStatusChanged?.Invoke(_isTyping); // Trigger the event
            }
        }
    }

    public event Action<bool> OnTypingStatusChanged; // Event for typing status changes

    private void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
        _fullText = _textComponent.text;
        _textComponent.text = string.Empty;
    }

    private IEnumerator TypeText()
    {
        _textComponent.text = string.Empty;
        IsTyping = true;

        bool insideTag = false;

        for (int i = 0; i < _fullText.Length; i++)
        {
            char letter = _fullText[i];

            if (letter == '<') insideTag = true;
            if (!insideTag)
            {
                _textComponent.text += letter;
                yield return new WaitForSeconds(_typingSpeed);
            }
            else
            {
                _textComponent.text += letter; // Add the tag characters immediately
            }

            if (letter == '>') insideTag = false;
        }

        IsTyping = false;
    }

    public void SetTypingSpeed(float newSpeed)
    {
        _typingSpeed = newSpeed;
    }

    public void SetFullText(string newText)
    {
        _fullText = newText;
        StartCoroutine(TypeText());
    }
}