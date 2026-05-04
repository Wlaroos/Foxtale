using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip _talkingMusic;
    [SerializeField] private AudioClip _minigameMusic;
    [SerializeField] private AudioClip _bossMusic;
    [SerializeField] private float _fadeDuration = 0.5f;
    private AudioSource _audioSource;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayTalkingMusic()
    {
        PlayMusic(_talkingMusic);
    }

    public void PlayMinigameMusic()
    {
        PlayMusic(_minigameMusic);
    }

    public void PlayBossMusic()
    {
        PlayMusic(_bossMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (_audioSource.clip == clip)
            return;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        
        _fadeCoroutine = StartCoroutine(FadeToNewMusicCoroutine(clip));
    }

    private IEnumerator FadeToVolume(float targetVolume, bool stopOnZero)
    {
        float startVolume = _audioSource.volume;
        for (float t = 0; t < _fadeDuration; t += Time.unscaledDeltaTime)
        {
            _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / _fadeDuration);
            yield return null;
        }
        _audioSource.volume = targetVolume;
        if (stopOnZero && Mathf.Approximately(targetVolume, 0f))
            _audioSource.Stop();
        _fadeCoroutine = null;
    }

    private IEnumerator FadeToNewMusicCoroutine(AudioClip newClip)
    {
        float prevVolume = _audioSource.volume;
        yield return StartCoroutine(FadeToVolume(0f, false));
        _audioSource.clip = newClip;
        _audioSource.Play();
        yield return StartCoroutine(FadeToVolume(prevVolume, false));
        _fadeCoroutine = null;
    }

}
