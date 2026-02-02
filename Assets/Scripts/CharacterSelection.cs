using System.Collections;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance;
    [SerializeField] private SpriteRenderer[] _podium;
    [SerializeField] private SpriteRenderer[] _pointHand;
    [SerializeField] private GameObject[] _grabHand;
    [SerializeField] private GameObject[] _handPoint;
    [SerializeField] private ParticleSystem _bloodEffect;
    [SerializeField] private ParticleSystem _boneEffect;

    private Color _defaultColor = Color.white;
    private Color _hoverColor = new Color32(255, 255, 255, 255);

    private SpriteRenderer _currentlyHoveredPodium;
    private SpriteRenderer _currentlyHoveredHand;

    private GameObject _hand1;
    private GameObject _hand2;

    [HideInInspector] public bool _selected = false;
    [HideInInspector] public bool _ready = false;

    private int _selectedCharacterIndex = -1; // Tracks the selected character (0 = Merlin, 1 = Walm, 2 = Runic)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!_selected && _ready)
        {
            HandleHover();
            HandleClick();
        }
    }

    private void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;

            for (int i = 0; i < _podium.Length; i++)
            {
                if (hitObject == _podium[i].gameObject)
                {
                    SetHoverState(_podium[i], _pointHand[i]);
                    return;
                }
            }
        }

        ResetHoverState();
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;

                for (int i = 0; i < _podium.Length; i++)
                {
                    if (hitObject == _podium[i].gameObject)
                    {
                        SelectCharacter(i);
                        return;
                    }
                }
            }
        }
    }

    private void SetHoverState(SpriteRenderer podium, SpriteRenderer hand)
    {
        ResetHoverState();

        _currentlyHoveredPodium = podium;
        _currentlyHoveredHand = hand;

        podium.color = _hoverColor;
        hand.gameObject.SetActive(true);
    }

    private void ResetHoverState()
    {
        if (_currentlyHoveredPodium != null)
        {
            _currentlyHoveredPodium.color = _defaultColor;
            _currentlyHoveredHand.gameObject.SetActive(false);

            _currentlyHoveredPodium = null;
            _currentlyHoveredHand = null;
        }
    }

    private void SelectCharacter(int characterIndex)
    {
        _selectedCharacterIndex = characterIndex;

        for (int i = 0; i < _podium.Length; i++)
        {
            if (i == characterIndex)
            {
                _podium[i].enabled = true;
            }
            else
            {
                Destroy(_podium[i].gameObject);
            }
        }

        for (int i = 0; i < _grabHand.Length; i++)
        {
            _grabHand[i].SetActive(i != characterIndex);
        }

        _hand1 = _grabHand[(characterIndex + 1) % _grabHand.Length];
        _hand2 = _grabHand[(characterIndex + 2) % _grabHand.Length];

        StartCoroutine(MoveHandToPoint(_hand1, _handPoint[0], Random.Range(0.5f, 1.5f)));
        StartCoroutine(MoveHandToPoint(_hand2, _handPoint[1], Random.Range(0.5f, 1.5f)));

        _selected = true;

        foreach (var hand in _pointHand)
        {
            hand.gameObject.SetActive(false);
        }

        FairyAnimation.Instance.ArmsUp();
        FairyAnimation.Instance.ChangeFace("Smile");
    }

    public void RandomlySelectCharacter()
    {
        SelectCharacter(Random.Range(0, _podium.Length));
    }

    public void Death1()
    {
        StartCoroutine(MoveHandToPointDEATH(_hand1, _handPoint[2], 0.2f));
    }

    public void Death2()
    {
        StartCoroutine(MoveHandToPointDEATH(_hand2, _handPoint[2], 0.2f));
    }

    public void Death3()
    {
        if (_selectedCharacterIndex >= 0 && _selectedCharacterIndex < _grabHand.Length)
        {
            _podium[_selectedCharacterIndex].enabled = false;
            _grabHand[_selectedCharacterIndex].SetActive(true);
            GameObject selectedHand = _grabHand[_selectedCharacterIndex];
            StartCoroutine(MoveHandToPointFINAL(selectedHand, _handPoint[3], 2f));
        }
    }

    private IEnumerator MoveHandToPoint(GameObject hand, GameObject point, float duration)
    {
        float elapsed = 0f;

        Rigidbody2D rb = hand.GetComponent<Rigidbody2D>();

        Vector3 startingPos = hand.transform.position;
        Vector3 targetPos = point.transform.position;

        while (elapsed < duration)
        {
            rb.MovePosition(Vector3.Lerp(startingPos, targetPos, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        hand.transform.position = targetPos;
    }

    private IEnumerator MoveHandToPointDEATH(GameObject hand, GameObject point, float duration)
    {
        float elapsed = 0f;

        Rigidbody2D rb = hand.GetComponent<Rigidbody2D>();

        Vector3 startingPos = hand.transform.position;
        Vector3 targetPos = point.transform.position;

        while (elapsed < duration)
        {
            rb.MovePosition(Vector3.Lerp(startingPos, targetPos, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        hand.transform.position = targetPos;

        Vector3 direction = (startingPos - targetPos).normalized;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

        Instantiate(_bloodEffect, hand.transform.position, rotation);
        Instantiate(_boneEffect, hand.transform.position, rotation);
        Instantiate(_boneEffect, hand.transform.position, rotation);
        Instantiate(_boneEffect, hand.transform.position, rotation);
        Instantiate(_boneEffect, hand.transform.position, rotation);
        Instantiate(_boneEffect, hand.transform.position, rotation);

        Destroy(hand);
    }

    private IEnumerator MoveHandToPointFINAL(GameObject hand, GameObject point, float duration)
    {
        float elapsed = 0f;

        Rigidbody2D rb = hand.GetComponent<Rigidbody2D>();

        Vector3 startingPos = hand.transform.position;
        Vector3 targetPos = point.transform.position;

        while (elapsed < duration)
        {
            rb.MovePosition(Vector3.Lerp(startingPos, targetPos, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        hand.transform.position = targetPos;

        FairyAnimation.Instance.ChangeFace("Evil");

        yield return new WaitForSeconds(3f);

        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 0));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 30));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 60));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 90));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 120));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 150));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 180));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 210));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 240));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 270));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 300));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 330));
        Instantiate(_bloodEffect, hand.transform.position, Quaternion.Euler(0, 0, 360));

        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 0));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 30));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 60));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 90));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 120));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 150));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 180));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 210));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 240));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 270));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 300));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 330));
        Instantiate(_boneEffect, hand.transform.position, Quaternion.Euler(0, 0, 360));

        Destroy(hand);

        yield return new WaitForSeconds(5f);

        Application.Quit();
    }
}
