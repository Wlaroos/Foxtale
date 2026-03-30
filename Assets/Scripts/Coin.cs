using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    private CoinManager _pool;
    [SerializeField] private float _lifetime = 5f;
    [Header("Homing")]
    [SerializeField] private float _homingSpeed = 30f;
    [SerializeField] private float _maxHomingStartDelay = 0.25f;
    private float _homingStartDelay = 0f;
    [SerializeField] private float _homingCollectDistance = 0.25f;
    [Header("Particles")]
    [SerializeField] private GameObject _coinParticlePrefab;

    private Transform _collector;

    private Coroutine _lifetimeRoutine;
    private Rigidbody _rb;
    private float _currentSpeed;
    private float _spawnTime;

    public void SetPool(CoinManager pool)
    {
        _pool = pool;
    }

    public void SetCollector(Transform collector)
    {
        _collector = collector;
    }

    void OnEnable()
    {
        if (_lifetimeRoutine != null) StopCoroutine(_lifetimeRoutine);
        _lifetimeRoutine = StartCoroutine(DisableAfter(_lifetime));
        _rb = GetComponent<Rigidbody>();
        _currentSpeed = _homingSpeed * 0.5f;
        _homingStartDelay = Random.Range(0f, _maxHomingStartDelay);
        _spawnTime = Time.time;
    }

    void OnDisable()
    {
        if (_lifetimeRoutine != null) StopCoroutine(_lifetimeRoutine);
        _lifetimeRoutine = null;
    }

    void Update()
    {
        if (_collector == null) return;

        if (Time.time - _spawnTime < _homingStartDelay) return;

        var dir = _collector.position - transform.position;
        float dist = dir.magnitude;

        if (dist <= _homingCollectDistance)
        {
            MinigameManager.Instance.AddMoney(1);
            Instantiate(_coinParticlePrefab, transform.position, Quaternion.identity);
            ReturnToPool();
            return;
        }

        var desiredSpeed = _homingSpeed;
        _currentSpeed = Mathf.MoveTowards(_currentSpeed, desiredSpeed, Time.deltaTime);

        if (_rb != null)
        {
            _rb.linearVelocity = dir.normalized * _currentSpeed;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _collector.position, _currentSpeed * Time.deltaTime);
        }
    }

    private IEnumerator DisableAfter(float t)
    {
        yield return new WaitForSeconds(t);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_lifetimeRoutine != null) { StopCoroutine(_lifetimeRoutine); _lifetimeRoutine = null; }
        if (_pool != null)
        {
            // reset velocity when returning to pool
            var rb = GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
            _pool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
