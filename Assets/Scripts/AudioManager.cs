using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;

    [HideInInspector] public float musicMasterVolume = 1f;
    [HideInInspector] public float sfxMasterVolume = 1f;

    public string[] whisperNames = { "Whisper1", "Whisper2", "Whisper3", "Whisper4", "Whisper5", "Whisper6" };
    public float minWhisperInterval = 5f;
    public float maxWhisperInterval = 12f;
    private bool isWhisperingActive = false;
    private string currentMusic;
    private Coroutine currentFadeRoutine;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = 0f; // 2D sound
            s.dynamicScale = 1f;
            UpdateSoundVolume(s);
        }
    }

    public void UpdateSoundVolume(Sound s)
    {
        if (s.source == null) return;

        float masterVol = 1f;
        if (s.type == AudioType.Music)
        {
            masterVol = musicMasterVolume;
        }
        else if (s.type == AudioType.SFX)
        {
            masterVol = sfxMasterVolume;
        }
        s.source.volume = s.volume * masterVol * s.dynamicScale;
    }

    public void SetMusicMasterVolume(float value)
    {
        musicMasterVolume = value;

        // Update semua lagu yang sedang main/diam
        foreach (Sound s in sounds)
        {
            if (s.type == AudioType.Music) UpdateSoundVolume(s);
        }
    }

    public void SetSFXMasterVolume(float value)
    {
        sfxMasterVolume = value;

        // Update semua SFX
        foreach (Sound s in sounds)
        {
            if (s.type == AudioType.SFX) UpdateSoundVolume(s);
        }
    }

    public void Start()
    {
        Play("OpeningTheme");
        Play("AmbienceWave");
        Play("AmbienceWind");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        UpdateSoundVolume(s);
        if (s.loop)
        {
            if (!s.source.isPlaying) //BGM tidak restart
                s.source.Play();
        }
        else
        {
            s.source.PlayOneShot(s.clip); // overlapping sound effect
        }
    }

    public void SetVolume(string name, float scale)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.dynamicScale = scale;
        UpdateSoundVolume(s);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void PlayMusicWithFade(string name, float fadeDuration)
    {
        if (currentMusic == name) return;

        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
        }

        currentFadeRoutine = StartCoroutine(FadeMusic(name, fadeDuration));
    }

    IEnumerator FadeMusic(string newMusic, float duration)
    {
        if (currentMusic == newMusic) yield break;

        Sound sNew = Array.Find(sounds, sound => sound.name == newMusic);
        Sound sOld = Array.Find(sounds, sound => sound.name == currentMusic);

        if (sNew == null)
        {
            Debug.LogWarning("Music: " + newMusic + " not found!");
            yield break;
        }

        sNew.source.volume = 0f;
        sNew.source.Play();

        float timer = 0f;
        float startVolOld = (sOld != null) ? sOld.source.volume : 0f;
        float targetVolNew = sNew.volume;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            if (sOld != null)
            {
                sOld.source.volume = Mathf.Lerp(startVolOld, 0f, progress);
            }

            sNew.source.volume = Mathf.Lerp(0f, targetVolNew, progress);

            yield return null;
        }

        if (sOld != null)
        {
            sOld.source.Stop();
            sOld.source.volume = sOld.volume;
        }

        sNew.source.volume = targetVolNew;
        currentMusic = newMusic;
    }

    public void StartWhispering()
    {
        if (!isWhisperingActive)
        {
            isWhisperingActive = true;
            StartCoroutine(WhisperRoutine());
        }
    }

    public void StopWhispering()
    {
        isWhisperingActive = false;
        StopCoroutine("WhisperRoutine");
    }

    IEnumerator WhisperRoutine()
    {
        while (isWhisperingActive)
        {
            float waitTime = UnityEngine.Random.Range(minWhisperInterval, maxWhisperInterval);
            yield return new WaitForSeconds(waitTime);

            if (whisperNames.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, whisperNames.Length);
                Play(whisperNames[randomIndex]);
            }
        }
    }
}
