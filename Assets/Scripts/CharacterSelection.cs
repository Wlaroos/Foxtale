using System.Collections;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _merlinPodium;
    [SerializeField] private SpriteRenderer _walmPodium;
    [SerializeField] private SpriteRenderer _runicPodium;
    [SerializeField] private SpriteRenderer _merlinPointHand;
    [SerializeField] private SpriteRenderer _walmPointHand;
    [SerializeField] private SpriteRenderer _runicPointHand;
    [SerializeField] private GameObject _merlinGrabHand;
    [SerializeField] private GameObject _walmGrabHand;
    [SerializeField] private GameObject _runicGrabHand;
    [SerializeField] private GameObject _handPoint01;
    [SerializeField] private GameObject _handPoint02;

    private Color _defaultColor = Color.white;
    private Color _hoverColor = new Color32(255,255,255,255);

    private SpriteRenderer _currentlyHoveredPodium;
    private SpriteRenderer _currentlyHoveredHand;

    private bool _selected = false;

    private void Update()
    {
        if(!_selected)
        {
            HandleHover();
            HandleClick();
        }
    }

    private void HandleHover()
    {
        // Cast a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;

            // Check which podium is being hovered over
            if (hitObject == _merlinPodium.gameObject)
            {
                SetHoverState(_merlinPodium, _merlinPointHand);
            }
            else if (hitObject == _walmPodium.gameObject)
            {
                SetHoverState(_walmPodium, _walmPointHand);
            }
            else if (hitObject == _runicPodium.gameObject)
            {
                SetHoverState(_runicPodium, _runicPointHand);
            }
        }
        else
        {
            // Reset hover state if no object is hit
            ResetHoverState();
        }
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;

                _merlinPodium.color = _defaultColor;
                _walmPodium.color = _defaultColor;
                _runicPodium.color = _defaultColor;

                // Check which podium is clicked
                if (hitObject == _merlinPodium.gameObject)
                {
                    SelectMerlin();
                }
                else if (hitObject == _walmPodium.gameObject)
                {
                    SelectWalm();
                }
                else if (hitObject == _runicPodium.gameObject)
                {
                    SelectRunic();
                }

                _selected = true;
                _merlinPointHand.gameObject.SetActive(false);
                _walmPointHand.gameObject.SetActive(false);
                _runicPointHand.gameObject.SetActive(false);

                FairyAnimation.Instance.ArmsUp();
                FairyAnimation.Instance.ChangeFace("Smile");
            }
        }
    }

    private void SetHoverState(SpriteRenderer podium, SpriteRenderer hand)
    {
        // Reset the previous hover state
        ResetHoverState();

        // Set the new hover state
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

    public void SelectMerlin()
    {
        _merlinPodium.enabled = true;
        Destroy(_walmPodium.gameObject);
        Destroy(_runicPodium.gameObject);

        _merlinGrabHand.SetActive(false);
        _walmGrabHand.SetActive(true);
        _runicGrabHand.SetActive(true);

        StartCoroutine(MoveHandToPoint(_walmGrabHand, _handPoint01));
        StartCoroutine(MoveHandToPoint(_runicGrabHand, _handPoint02));
    }

    public void SelectWalm()
    {
        Destroy(_merlinPodium.gameObject);
        _walmPodium.enabled = true;
        Destroy(_runicPodium.gameObject);

        _merlinGrabHand.SetActive(true);
        _walmGrabHand.SetActive(false);
        _runicGrabHand.SetActive(true);

        StartCoroutine(MoveHandToPoint(_merlinGrabHand, _handPoint01));
        StartCoroutine(MoveHandToPoint(_runicGrabHand, _handPoint02));
    }

    public void SelectRunic()
    {
        Destroy(_merlinPodium.gameObject);
        Destroy(_walmPodium.gameObject);
        _runicPodium.enabled = true;

        _merlinGrabHand.SetActive(true);
        _walmGrabHand.SetActive(true);
        _runicGrabHand.SetActive(false);

        StartCoroutine(MoveHandToPoint(_merlinGrabHand, _handPoint01));
        StartCoroutine(MoveHandToPoint(_walmGrabHand, _handPoint02));
    }

    private IEnumerator MoveHandToPoint(GameObject hand, GameObject point)
    {
        float duration = Random.Range(0.5f, 1.5f); // Duration of the movement
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
}
