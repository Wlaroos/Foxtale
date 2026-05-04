using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioClip _buttonClickSFX;
    [SerializeField] private AudioClip _boneClickSFX;
    [SerializeField] private AudioClip _boneBreakSFX;
    [SerializeField] private AudioClip _coinCollectSFX;
    [SerializeField] private AudioClip _minigameWinSFX;
    [SerializeField] private AudioClip _minigameLoseSFX;
    [SerializeField] private AudioClip _rotateClickSFX;
    [SerializeField] private AudioClip _arrowClickSFX;
    [SerializeField] private AudioClip _hurtSFX;
    [SerializeField] private AudioClip _deathSFX;
    [SerializeField] private AudioClip _attackSFX;
    [SerializeField] private AudioClip _heatbeatSFX;
    [SerializeField] private AudioClip _minigameLose2SFX;
    [SerializeField] private AudioClip _catSFX;
    [SerializeField] private AudioClip _cat2SFX;
    [SerializeField] private AudioClip _soulPopSFX;

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

    public void PlayButtonClick() =>  PlaySFX(_buttonClickSFX,1f, 0.3f);
    public void PlayCoinCollect() => PlaySFX(_coinCollectSFX, 0.5f, 0.1f);
    public void PlayMinigameWin() => PlaySFX(_minigameWinSFX, 1f, 0f);
    public void PlayMinigameLose() => PlaySFX(_minigameLoseSFX, 1f, 0f);
    public void PlayBoneClick() => PlaySFX(_boneClickSFX, 1f, 0.3f);
    public void PlayBoneBreak() => PlaySFX(_boneBreakSFX, 1f, 0.2f);
    public void PlayArrowClick() => PlaySFX(_arrowClickSFX, 1f, 0.2f);
    public void PlayRotateClick() => PlaySFX(_rotateClickSFX, 1f, 0.2f);
    public void PlayHurt() => PlaySFX(_hurtSFX, 1f, 0.2f);
    public void PlayDeath() => PlaySFX(_deathSFX, 1f, 0.2f);
    public void PlayAttack() => PlaySFX(_attackSFX, 1f, 0.2f);
    public void PlayHeartbeat() => PlaySFX(_heatbeatSFX, 1f, 0.2f);
    public void PlayMinigameLose2() => PlaySFX(_minigameLose2SFX, 1f, 0.2f);
    public void PlayCat() => PlaySFX(_catSFX, 1f, 0.3f);
    public void PlayCat2() => PlaySFX(_cat2SFX, 0.5f, 0.3f);
    public void PlaySoulPop() => PlaySFX(_soulPopSFX, 1f, 0.2f);
    private void PlaySFX(AudioClip clip, float volume = 1f, float pitchVariance = 0.1f)
    {
        if (clip == null) return;
        var source = GetPooledAudioSource();
        source.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }
}
