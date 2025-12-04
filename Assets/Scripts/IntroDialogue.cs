using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntroDialogue : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public CanvasGroup introPanel;
    public WaveManager waveManager;
    public Button skipButton;

    [TextArea(3,6)]
    public List<string> lines;

    public float typingSpeed = 0.03f;
    public float delayBetweenLines = 1f;

    bool skipping = false;

    void Start()
    {
        waveManager.enabled = false;
        skipButton.onClick.AddListener(SkipIntro);
        StartCoroutine(PlayIntro());
    }

    void SkipIntro()
    {
        skipping = true;
    }

    IEnumerator PlayIntro()
    {
        foreach (string line in lines)
        {
            if (skipping) break;

            yield return StartCoroutine(TypeSentence(line));

            if (skipping) break;

            yield return new WaitForSeconds(delayBetweenLines);
            textUI.text = "";
        }

        // langsung ke fade out
        yield return StartCoroutine(FadeOutPanel());

        waveManager.enabled = true;
    }

    IEnumerator TypeSentence(string sentence)
    {
        textUI.text = "";

        foreach (char c in sentence)
        {
            if (skipping) yield break;

            textUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator FadeOutPanel()
    {
        skipButton.gameObject.SetActive(false);

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            introPanel.alpha = 1 - t;
            yield return null;
        }

        introPanel.gameObject.SetActive(false);
    }
}
