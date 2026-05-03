using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioClip _buttonClickSFX;
    [SerializeField] private AudioClip _coinCollectSFX;
    [SerializeField] private AudioClip _minigameWinSFX;
    [SerializeField] private AudioClip _minigameLoseSFX;

    [SerializeField] private int _initialPoolSize = 10;
    [SerializeField] private int _maxPoolSize = 25;

    private Queue<AudioSource> _audioSourcePool;
    private List<AudioSource> _activeAudioSources;
    private Transform _poolParent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        _audioSourcePool = new Queue<AudioSource>(_initialPoolSize);
        _activeAudioSources = new List<AudioSource>(_initialPoolSize);
        _poolParent = new GameObject("SFXAudioSourcePool").transform;
        _poolParent.SetParent(transform);
        _poolParent.localPosition = Vector3.zero;

        for (int i = 0; i < _initialPoolSize; i++)
        {
            var source = CreateNewAudioSource();
            _audioSourcePool.Enqueue(source);
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        var go = new GameObject("PooledAudioSource");
        go.transform.SetParent(_poolParent);
        var source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    private AudioSource GetPooledAudioSource()
    {
        // Clean up finished sources
        for (int i = _activeAudioSources.Count - 1; i >= 0; i--)
        {
            if (!_activeAudioSources[i].isPlaying)
            {
                _audioSourcePool.Enqueue(_activeAudioSources[i]);
                _activeAudioSources.RemoveAt(i);
            }
        }

        if (_audioSourcePool.Count > 0)
        {
            var src = _audioSourcePool.Dequeue();
            _activeAudioSources.Add(src);
            return src;
        }
        else if (_activeAudioSources.Count + _audioSourcePool.Count < _maxPoolSize)
        {
            var src = CreateNewAudioSource();
            _activeAudioSources.Add(src);
            return src;
        }
        else
        {
            // If pool is exhausted, reuse the first available (may cut off sound)
            var src = _activeAudioSources[0];
            src.Stop();
            _activeAudioSources.RemoveAt(0);
            _activeAudioSources.Add(src);
            return src;
        }
    }

    public void PlayButtonClick()
    {
        PlaySFX(_buttonClickSFX);
    }

    public void PlayCoinCollect()
    {
        PlaySFX(_coinCollectSFX);
    }

    public void PlayMinigameWin()
    {
        PlaySFX(_minigameWinSFX);
    }

    public void PlayMinigameLose()
    {
        PlaySFX(_minigameLoseSFX);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        var source = GetPooledAudioSource();
        source.clip = clip;
        source.Play();
    }
}
