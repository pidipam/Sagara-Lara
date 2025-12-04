
using System.Collections;
using UnityEngine;
using TMPro;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

    [Header("UI")]
    public TextMeshProUGUI comboText;

    [Header("Settings")]
    public float comboResetTime = 3f;
    public float bounceDuration = 0.3f;
    public float fadeDuration = 0.5f;

    private int comboCount = 0;
    private float timer = 0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (comboCount > 0)
        {
            timer += Time.deltaTime;
            if (timer >= comboResetTime)
                ResetCombo();
        }
    }

    public void AddCombo()
    {
        comboCount++;
        timer = 0f;

        comboText.gameObject.SetActive(true);

        // Reset alpha agar tidak transparan
        Color c = comboText.color;
        c.a = 1f;
        comboText.color = c;

        // Warna dinamis (semakin besar combo → semakin merah)
        comboText.color = Color.Lerp(Color.white, Color.red, Mathf.Clamp01(comboCount / 10f));

        comboText.text = comboCount + "x COMBO!";

        StopAllCoroutines();
        StartCoroutine(BounceEffect());

        if (AudioManager.instance != null)
        {
            string soundName = "";
            float pitch = 1f;

            if (comboCount <= 6)
            {
                soundName = "Combo" + comboCount;
                pitch = 1f;
            }
            else
            {
                soundName = "Combo6";
                int stepOver = comboCount - 6;
                pitch = 1f + (stepOver * 0.1f);

                pitch = Mathf.Clamp(pitch, 1f, 3f);
            }
            PlayComboSound(soundName, pitch);
        }
    }

    void PlayComboSound(string soundName, float pitch)
    {
        Sound s = System.Array.Find(AudioManager.instance.sounds, item => item.name == soundName);
        if (s != null)
        {
            s.source.pitch = pitch;
            s.source.PlayOneShot(s.clip);
        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
        timer = 0f;

        if (AudioManager.instance != null)
        {
            Sound s = System.Array.Find(AudioManager.instance.sounds, item => item.name == "Combo6");
            if (s != null)
            {
                s.source.pitch = 1f;
            }
        }
        StopAllCoroutines();
        StartCoroutine(FadeOutEffect());
    }

    public int GetCombo()
    {
        return comboCount;
    }

    IEnumerator BounceEffect()
    {
        comboText.transform.localScale = Vector3.one;

        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * 1.3f;

        // Scale up
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            comboText.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // Scale back
        elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            comboText.transform.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }
    }

    IEnumerator FadeOutEffect()
    {
        Color startColor = comboText.color;
        Color targetColor = startColor;
        targetColor.a = 0f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            comboText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        comboText.gameObject.SetActive(false);
        comboText.color = startColor; // Reset alpha untuk next combo
    }
}
