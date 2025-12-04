
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Multiplier Settings")]
    public float comboMultiplierStep = 0.1f; // Setiap combo naik, multiplier naik 0.1

    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int baseScore)
    {
        int comboCount = ComboManager.Instance.GetCombo();
        float multiplier = 1f + (comboCount * comboMultiplierStep);

        int finalScore = Mathf.RoundToInt(baseScore * multiplier);
        score += finalScore;

        if (scoreText != null)
            scoreText.text = "Score: " + score;

        Debug.Log($"Score Added: {finalScore} (Multiplier: {multiplier})");
    }
}
