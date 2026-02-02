using UnityEngine;

public class FairyText : MonoBehaviour
{
    [SerializeField] private string[] fairyMessage = 
    { 
        "Choose your character",
        "",
        "",
        "",
        "" 
    };
    private TypewriterEffect _typewriterRef;
    private int currentMessageIndex = 0;
    private void Awake()
    {
        _typewriterRef = GetComponent<TypewriterEffect>();

        if (_typewriterRef == null)
        {
            Debug.LogError("TypewriterEffect component is missing.");
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
    }

    private void NextMessage()
    {
        if (_typewriterRef != null && currentMessageIndex < fairyMessage.Length && !_typewriterRef._isTyping)
        {
            _typewriterRef.SetFullText(fairyMessage[currentMessageIndex]);

            currentMessageIndex++;
        }
    }
}
