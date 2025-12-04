using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public float speed = 1.5f;
    public float rotationSpeed = 5f;
    public TextMeshPro wordText;

    public System.Action<EnemyController, bool> OnDeath;

    public GameObject explosionPrefabTyping;
    public GameObject explosionPrefabPlayer;

    private string word;
    private int currentIndex = 0;
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    public void SetWord(string newWord)
    {
        word = newWord.Trim();
        wordText.text = word;
    }

    public string GetWord()
    {
        return word;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(1);

            ComboManager.Instance.ResetCombo();

            Die(explosionPrefabPlayer, false);
        }
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90f);

            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public bool TypeLetter(char letter)
    {
        while (currentIndex < word.Length && word[currentIndex] == ' ')
            currentIndex++;

        if (currentIndex < word.Length &&
            char.ToLower(word[currentIndex]) == char.ToLower(letter))
        {
            currentIndex++;

            while (currentIndex < word.Length && word[currentIndex] == ' ')
                currentIndex++;

            wordText.text = $"<color=green>{word.Substring(0, currentIndex)}</color>{word.Substring(currentIndex)}";

            if (currentIndex >= word.Length)
            {
                ScoreManager.Instance.AddScore(1);

                // Heal combo player
                PlayerHealth hp = FindObjectOfType<PlayerHealth>();
                if (hp != null)
                    hp.RegisterKill();

                // Tambah combo
                ComboManager.Instance.AddCombo();

                Die(explosionPrefabTyping, true);
                return true;
            }
        }

        return false;
    }

    private void Die(GameObject explosionPrefab, bool killedByTyping)
    {
        OnDeath?.Invoke(this, killedByTyping);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("EnemyDeath");
        }

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    // ==========================================================
    //                Fungsi tambahan buat AIM
    // ==========================================================

    public bool IsCorrectLetter(char letter)
    {
        int tempIndex = currentIndex;

        while (tempIndex < word.Length && word[tempIndex] == ' ')
            tempIndex++;

        if (tempIndex < word.Length)
            return char.ToLower(word[tempIndex]) == char.ToLower(letter);

        return false;
    }

    public bool IsFinished()
    {
        return currentIndex >= word.Length;
    }
}
