using UnityEngine;

public class LevelMusicPlayer : MonoBehaviour
{
    [Header("Audio Clip")]
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private float fadeDuration = 2f;

    private void Start()
    {
        if (AudioManager.Instance != null && levelMusic != null)
        {
            AudioManager.Instance.ChangeMusicWithFade(levelMusic, fadeDuration);
        }
    }
}