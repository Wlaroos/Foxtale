using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHoverUtil : MonoBehaviour
{
    [SerializeField] private bool _usesLocalTime = true;
    private float _localTime;

    [SerializeField] private bool _canRotate;
    [SerializeField] private Vector3 _degreesPerSecond = new Vector3(0f,0f,15f);

    [SerializeField] private bool _canHover;
    // Amount the object will go up and down. Range of movement is (position - amplitude) to (position + amplitude)
    [SerializeField] private float _amplitude = 0.5f;
    public float Amplitude => _amplitude;
    // Time needed for one full cycle
    [SerializeField] private float _frequency = 1f;

    private Vector3 _posOffset = new Vector3();
    private Vector3 _tempPos = new Vector3();

    private float _sine;
    public float Sine => _sine;

    void Awake()
    {
        Setup();
    }

    private void OnEnable()
    {
        Setup();
    }

    void Update()
    {
        if(_usesLocalTime)
        {
            _localTime += Time.deltaTime;
        }
        else
        {
            _localTime = Time.fixedTime;
        }

        if (_canRotate)
        {
            transform.Rotate(Time.deltaTime * _degreesPerSecond, Space.Self);
        }

        if (_canHover)
        {
            _tempPos = _posOffset;

            _sine = Mathf.Sin(_localTime * Mathf.PI * _frequency);
            _tempPos.y += (_sine * _amplitude);

            transform.localPosition = _tempPos;
        }
    }

    void Setup()
    {
        _posOffset = transform.localPosition;

        if(_usesLocalTime)
        {
            _localTime = Random.Range(0,1000);
            _frequency = Random.Range(_frequency, _frequency+.25f);
        }
    }

    public void NewPosistionOffset()
    {
        _posOffset = transform.localPosition;
    }
}
