using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float _typingSpeed = 0.05f;
    private TMP_Text _textComponent;
    private string _fullText;
    [HideInInspector] public bool _isTyping;

    private void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
        _fullText = _textComponent.text;
        _textComponent.text = string.Empty;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isTyping)
        {
            StopAllCoroutines();
            _textComponent.text = _fullText;
            _isTyping = false;
        }

        if (Input.GetMouseButtonDown(1) && _isTyping)
        {
            _typingSpeed = 0.01f;
        }
        else if (Input.GetMouseButtonUp(1) && _isTyping)
        {
            _typingSpeed = 0.05f;
        }
    }

    private IEnumerator TypeText()
    {
        _textComponent.text = string.Empty;

        _isTyping = true;
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

        _isTyping = false;
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