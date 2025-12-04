using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class ChapterDisplay : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;
    public float displayDuration = 3f;

    public IEnumerator ShowChapterRoutine(string title, string subtitle)
    {
        titleText.text = title;
        subtitleText.text = subtitle;
        titleText.gameObject.SetActive(true);
        subtitleText.gameObject.SetActive(true);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        titleText.gameObject.SetActive(false);
        subtitleText.gameObject.SetActive(false);
    }
}

