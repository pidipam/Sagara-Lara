
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class WordInput : MonoBehaviour
{
    private PlayerController player;
    private EnemyController activeEnemy;

    [Header("UI")]
    public TextMeshProUGUI wrongInputText; // Assign di Inspector
    public float bounceDuration = 0.2f;
    public float fadeDuration = 0.4f;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        if (wrongInputText != null)
            wrongInputText.gameObject.SetActive(false);
    }

    void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (!char.IsLetter(c)) continue;

            // Cari target baru jika belum ada
            if (activeEnemy == null)
            {
                activeEnemy = FindObjectsOfType<EnemyController>()
                    .Where(e => e.GetWord().TrimStart()
                        .StartsWith(c.ToString(), System.StringComparison.OrdinalIgnoreCase))
                    .OrderBy(e => Vector3.Distance(player.transform.position, e.transform.position))
                    .FirstOrDefault();

                if (activeEnemy != null)
                    player.SetTarget(activeEnemy);
            }

            if (activeEnemy != null)
            {
                bool correctLetter = activeEnemy.IsCorrectLetter(c);

                if (correctLetter)
                {
                    player.FireAtTarget();
                }
                else
                {
                    ShowWrongIndicator(); // Tampilkan indikator salah ketik
                }

                activeEnemy.TypeLetter(c);

                // jika kata selesai → reset target
                if (activeEnemy.IsFinished())
                {
                    // Tambahkan combo


                    // Tambahkan score dengan multiplier
                    ScoreManager.Instance.AddScore(1); // Base score

                    activeEnemy = null;
                    player.SetTarget(null);
                }
            }
        }
    }

    void ShowWrongIndicator()
    {
        if (wrongInputText == null) return;

        wrongInputText.gameObject.SetActive(true);
        wrongInputText.text = "TETOT!";
        wrongInputText.color = Color.red;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("Tetot");
        }

        if (ComboManager.Instance != null)
        {
            ComboManager.Instance.ResetCombo();
        }

        StopAllCoroutines();
        StartCoroutine(BounceAndFadeWrong());
    }

    IEnumerator BounceAndFadeWrong()
    {
        // Reset scale dan alpha
        wrongInputText.transform.localScale = Vector3.one;
        Color startColor = wrongInputText.color;
        startColor.a = 1f;
        wrongInputText.color = startColor;

        // Bounce (scale up lalu kembali)
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * 1.3f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            wrongInputText.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            wrongInputText.transform.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }

        // Fade out
        elapsed = 0f;
        Color targetColor = startColor;
        targetColor.a = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            wrongInputText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        wrongInputText.gameObject.SetActive(false);
    }
}
