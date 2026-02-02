using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;

    [RequireComponent(typeof(TMP_Text))]
    public class TypewriterEffect : MonoBehaviour
    {
        [SerializeField] private float typingSpeed = 0.05f;
        private TMP_Text _textComponent;
        private string _fullText;
        private Coroutine _typingCoroutine;

        private void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();
            _fullText = _textComponent.text;
            _textComponent.text = string.Empty;
        }

        private void OnEnable()
        {
            StartTyping();
        }

        public void StartTyping()
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }
            _typingCoroutine = StartCoroutine(TypeText());
        }

        private IEnumerator TypeText()
        {
            _textComponent.text = string.Empty;
            foreach (char letter in _fullText)
            {
                _textComponent.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        public void SetTypingSpeed(float newSpeed)
        {
            typingSpeed = newSpeed;
        }

        public void SetFullText(string newText)
        {
            _fullText = newText;
            StartTyping();
        }
    }