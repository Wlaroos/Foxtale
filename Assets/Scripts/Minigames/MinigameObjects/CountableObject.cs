using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class CountableObject : MonoBehaviour
{
    [SerializeField] private Sprite[] _fingerSprites; // Array of sprites representing 0 to 5 fingers
    [HideInInspector] public bool _isInteractable = true;
    private ParticleSystem _clickEffect;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _clickCollider;
    
    private int _currentValue = 0;
    public int CurrentValue => _currentValue;
    
    public event Action<CountableObject> OnValueChanged;

    private void Awake()
    {
        AssignReferences();
        UpdateVisuals();
    }

    private void AssignReferences()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        _clickCollider = GetComponent<Collider2D>();

        _clickEffect = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!_isInteractable) return;

        // Check for left click (0) or right click (1)
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (_clickCollider != null && _clickCollider.OverlapPoint(mousePos))
            {
                // Left click increases, right click decreases
                ChangeValue(Input.GetMouseButtonDown(0));
            }
        }
    }

    private void ChangeValue(bool increase)
    {
        _clickEffect.Play();
        _clickEffect.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));

        if (increase)
        {
            // Increase value (max 5 fingers per hand)
            _currentValue = (_currentValue + 1) % 6; 
        }
        else
        {
            // Decrease value (min 0 fingers)
            _currentValue--;
            if (_currentValue < 0) _currentValue = 5;
        }

        UpdateVisuals();
        OnValueChanged?.Invoke(this);

        SFXManager.Instance.PlayRotateClick();
    }

    private void UpdateVisuals()
    {
        if (_fingerSprites != null && _fingerSprites.Length > _currentValue && _spriteRenderer != null)
        {
            _spriteRenderer.sprite = _fingerSprites[_currentValue];
        }
    }
}