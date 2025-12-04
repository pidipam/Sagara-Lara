using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Components")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 1f);

        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        ApplyMusicVolume(savedMusic);
        ApplySFXVolume(savedSFX);

        if (musicSlider != null) musicSlider.onValueChanged.AddListener(ApplyMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(ApplySFXVolume);
    }

    public void ApplyMusicVolume(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetMusicMasterVolume(value);
        }
        PlayerPrefs.SetFloat("MusicVol", value);
    }

    public void ApplySFXVolume(float value)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetSFXMasterVolume(value);
        }
        PlayerPrefs.SetFloat("SFXVol", value);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
}