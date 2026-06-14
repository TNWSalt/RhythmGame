using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public static MusicManager GetInstance() { return instance; }

    [SerializeField] private AudioSource audioSource;
    private bool isPaused;

    public float PlaybackTime => audioSource != null ? audioSource.time : 0f;
    public bool HasClip => audioSource != null && audioSource.clip != null;
    public bool IsPlaying => audioSource != null && audioSource.isPlaying;

    public event Action OnMusicFinished;

    private void Awake()
	{
		if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

	public void PlayMusic(AudioClip clip, bool loop = false)
    {
        if (audioSource == null) { return; }

        StopAllCoroutines();
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        isPaused = false;

        if (clip == null)
        {
            return;
        }

        audioSource.time = 0f;
        audioSource.Play();
        // Start finish watcher only when a real clip is playing.
        if (!loop)
        {
            StartCoroutine(WaitForMusicEnd());
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
        isPaused = false;
        StopAllCoroutines();
    }

    private IEnumerator WaitForMusicEnd()
    {
        // 等到不再播放
        yield return new WaitWhile(() => audioSource.isPlaying || isPaused);

        // 呼叫回調事件
        OnMusicFinished?.Invoke();
    }

    public void SetAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public void PauseMusic()
    {
        if (audioSource == null || !audioSource.isPlaying) { return; }
        isPaused = true;
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (audioSource == null || !isPaused) { return; }
        isPaused = false;
        audioSource.UnPause();
    }
}
