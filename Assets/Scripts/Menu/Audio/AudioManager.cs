using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip buttonClickClip;

    private float currentMusicVolume = 0.5f;
    private float currentSFXVolume = 0.5f;

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
            return;
        }
    }

    private void Start()
    {
        if (musicSource != null) musicSource.volume = 0f;
        if (sfxSource != null) sfxSource.volume = currentSFXVolume;
    }

    public void PlayButtonClick()
    {
        if (buttonClickClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickClip, currentSFXVolume);
        }
    }

    public void FadeOutMusic(float duration = 0.6f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(duration));
    }

    public void ChangeMusicWithFade(AudioClip newClip, float fadeDuration = 1f)
    {
        if (musicSource == null) return;
        if (musicSource.clip == newClip && musicSource.isPlaying && musicSource.volume > 0.05f) return;

        StopAllCoroutines();
        StartCoroutine(FadeMusicRoutine(newClip, fadeDuration));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        if (musicSource == null || !musicSource.isPlaying) yield break;

        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }

    private IEnumerator FadeMusicRoutine(AudioClip newClip, float duration)
    {
        if (musicSource.isPlaying && musicSource.volume > 0f)
        {
            float startVolume = musicSource.volume;
            float elapsedOut = 0f;
            float halfDuration = duration * 0.4f;

            while (elapsedOut < halfDuration)
            {
                elapsedOut += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsedOut / halfDuration);
                yield return null;
            }
        }

        musicSource.volume = 0f;
        musicSource.clip = newClip;

        if (newClip != null)
        {
            musicSource.loop = true;
            musicSource.Play();

            float elapsedIn = 0f;
            float fadeInDuration = duration * 0.6f;

            while (elapsedIn < fadeInDuration)
            {
                elapsedIn += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(0f, currentMusicVolume, elapsedIn / fadeInDuration);
                yield return null;
            }
            musicSource.volume = currentMusicVolume;
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = volume;
        if (musicSource != null) musicSource.volume = currentMusicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        currentSFXVolume = volume;
        if (sfxSource != null) sfxSource.volume = currentSFXVolume;
    }

    public float GetMusicVolume() => currentMusicVolume;
    public float GetSFXVolume() => currentSFXVolume;
}