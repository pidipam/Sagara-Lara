
using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public CanvasGroup ui;
    public TextMeshProUGUI dialogueText;
    public GameObject skipButton;

    public enum CutsceneType { Opening, Ending }
    private CutsceneType currentType;

    [TextArea(3, 10)]
    public string[] openingLines;

    [TextArea(3, 10)]
    public string[] endingLines;

    private string[] activeLines;
    private bool isActive = false;

    public System.Action OnCutsceneFinished;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;       // kecepatan ketik per huruf
    public float delayBetweenLines = 1f;    // jeda antar kalimat
    public string typingSoundName = "Typing";

    // ============================================================
    //  PUBLIC API
    // ============================================================
    public void PlayOpening()
    {
        currentType = CutsceneType.Opening;
        activeLines = openingLines;
        StartCoroutine(Play());
    }

    public void PlayEnding()
    {
        currentType = CutsceneType.Ending;
        activeLines = endingLines;
        StartCoroutine(Play());
    }

    // ============================================================
    //  PLAY CUTSCENE
    // ============================================================
    IEnumerator Play()
    {
        isActive = true;

        ui.alpha = 1f;
        ui.blocksRaycasts = true;

        skipButton.SetActive(true);
        dialogueText.text = "";

        yield return new WaitForSeconds(0.5f);

        foreach (string line in activeLines)
        {
            yield return StartCoroutine(TypeSentence(line)); // efek typing
            yield return new WaitForSeconds(delayBetweenLines); // jeda antar kalimat
            dialogueText.text = "";
        }

        Finish();
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char c in sentence)
        {
            dialogueText.text += c;
            if (c != ' ' && AudioManager.instance != null)
            {
                AudioManager.instance.Play(typingSoundName);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void Skip()
    {
        if (!isActive) return;
        Finish();
    }

    void Finish()
    {
        StopAllCoroutines();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Stop(typingSoundName);
        }

        isActive = false;

        ui.alpha = 0;
        ui.blocksRaycasts = false;
        skipButton.SetActive(false);

        OnCutsceneFinished?.Invoke();
        OnCutsceneFinished = null;
    }
}
