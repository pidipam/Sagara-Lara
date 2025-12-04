using UnityEngine.Audio;
using UnityEngine;

public enum AudioType
{
    Music,
    SFX
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    public AudioType type;

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    [HideInInspector]
    public float dynamicScale = 1f;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
