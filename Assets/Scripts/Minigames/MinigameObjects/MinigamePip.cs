using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MinigamePip : MonoBehaviour
{
    public enum PipShape { Middle, Bottom, Big }
    public PipShape CurrentShape { get; private set; }

    [Header("Shapes")]
    [SerializeField] private Sprite _middleSprite;
    [SerializeField] private Sprite _bottomSprite;
    [SerializeField] private Sprite _bigSprite;

    private Color _greyColor = new Color32(50, 50, 50, 255);
    private Color _whiteColor = Color.white;
    private Color _greenColor = Color.green;
    private Color _redColor = Color.red;
    private Color _yellowColor = Color.yellow;

    private Image _image;
    private bool _isCurrentPip = false;
    private bool _isFinished = false;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public void Initialize(PipShape shape)
    {
        switch (shape)
        {
            case PipShape.Middle: _image.sprite = _middleSprite; break;
            case PipShape.Bottom: _image.sprite = _bottomSprite; break;
            case PipShape.Big:    _image.sprite = _bigSprite;    break;
        }
        _image.color = _greyColor;
        _image.SetNativeSize();

        CurrentShape = shape;
    }

    public void SetActive(bool active, bool tutorial)
    {
        if (_isFinished) return; // Don't flash if already won
        
        StopAllCoroutines();
        _isCurrentPip = active;

        if (_isCurrentPip)
            StartCoroutine(FlashRoutine(tutorial));
        else
            _image.color = _greyColor;
    }

    public void SetWin()
    {
        _isFinished = true;
        _isCurrentPip = false;
        StopAllCoroutines();
        _image.color = _greenColor;
    }

    public void SetFail()
    {
        _isFinished = true;
        _isCurrentPip = false;
        StopAllCoroutines();
        _image.color = _redColor;
    }

    private IEnumerator FlashRoutine(bool tutorial)
    {
        while (_isCurrentPip)
        {
            if(tutorial)
            {
                _image.color = _yellowColor;
            }
            else
            {
                _image.color = _whiteColor;
            }
            yield return new WaitForSeconds(0.3f);
            _image.color = _greyColor;
            yield return new WaitForSeconds(0.3f);
        }
    }
}