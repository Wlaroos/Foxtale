using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private Transform _coinCollector;
    [SerializeField] private int _initialPool = 10;
    [SerializeField] private float _spawnRadius = 2f;
    [SerializeField] private float _throwForce = 2f;
    private Queue<GameObject> _pool = new Queue<GameObject>();

    void Awake()
    {
        if (_coinPrefab == null) return;

        for (int i = 0; i < _initialPool; i++)
        {
            var g = Instantiate(_coinPrefab, transform);
            g.SetActive(false);
            var c = g.GetComponent<Coin>();
            if (c != null) c.SetPool(this);
            _pool.Enqueue(g);
        }
    }

    public void CreateCoins(int amount, float delay = 0.1f)
    {
        StartCoroutine(CreateCoinsWithDelay(amount, delay));
    }

    private IEnumerator CreateCoinsWithDelay(int amount, float delay)
    {
        if (_coinPrefab == null)
        {
            Debug.LogWarning("CoinManager: coinPrefab not assigned.");
            yield break;
        }

        for (int i = 0; i < amount; i++)
        {
            var g = GetFromPool();
            var pos = transform.position + (Vector3)(Random.insideUnitCircle * _spawnRadius);
            g.transform.position = pos;
            g.transform.rotation = Quaternion.identity;
            g.SetActive(true);

            var rb = g.GetComponent<Rigidbody>();
            if (rb != null)
            {
                var dir = new Vector3(Random.Range(-1f, 1f), Random.Range(0.4f, 1f), Random.Range(-1f, 1f)).normalized;
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(dir * _throwForce, ForceMode.VelocityChange);
            }

            var coinComp = g.GetComponent<Coin>();
            if (coinComp != null)
            {
                coinComp.SetCollector(_coinCollector);
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private GameObject GetFromPool()
    {
        GameObject g;
        if (_pool.Count > 0)
        {
            g = _pool.Dequeue();
        }
        else
        {
            g = Instantiate(_coinPrefab, transform);
            var c = g.GetComponent<Coin>();
            if (c != null)
            {
                c.SetPool(this);
                c.SetCollector(_coinCollector);
            }
        }

        return g;
    }

    public void Release(GameObject go)
    {
        go.SetActive(false);
        _pool.Enqueue(go);
    }

    // Optional: ensure pool has at least `size` inactive objects
    public void EnsurePoolSize(int size)
    {
        if (_coinPrefab == null) return;
        while (_pool.Count < size)
        {
            var g = Instantiate(_coinPrefab, transform);
            g.SetActive(false);
            var c = g.GetComponent<Coin>();
            if (c != null)
            {
                c.SetPool(this);
                c.SetCollector(_coinCollector);
            }
            _pool.Enqueue(g);
        }
    }
}
