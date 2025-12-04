using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Tentacles")]
    public Transform[] tentacles;

    [Header("Enemy Spawn")]
    public GameObject sentenceEnemyPrefab;
    public WordBankBoss bossWords;

    [Header("Timing")]
    public float attackInterval = 3f;

    [Header("Effects")]
    public GameObject tentacleExplosionPrefab;
    public GameObject bossExplosionPrefab;
    public Animator bossAnimator;
    public float destroyDelay = 1.5f;

    private int tentaclesRemaining;

    void Start()
    {
        tentaclesRemaining = tentacles.Length;
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (tentaclesRemaining > 0)
        {
            int idx = Random.Range(0, tentacles.Length);

            if (tentacles[idx] != null)
            {
                SpawnSentence(tentacles[idx]);
                yield return new WaitForSeconds(attackInterval);
            }
            else
            {
                yield return null;
            }
        }

        BossDefeated();
    }

    void SpawnSentence(Transform point)
    {
        GameObject enemy = Instantiate(sentenceEnemyPrefab, point.position, Quaternion.identity);

        EnemyController ec = enemy.GetComponent<EnemyController>();
        ec.SetWord(bossWords.GetRandomWord());

        ec.OnDeath += SentenceDestroyed;
    }

    void SentenceDestroyed(EnemyController ec, bool killedByTyping)
    {
        if (!killedByTyping) return;

        for (int i = 0; i < tentacles.Length; i++)
        {
            if (tentacles[i] != null &&
                Vector3.Distance(tentacles[i].position, ec.transform.position) < 7.0f)
            {
                if (tentacleExplosionPrefab != null)
                {
                    Instantiate(tentacleExplosionPrefab, tentacles[i].position, Quaternion.identity);
                }

                Destroy(tentacles[i].gameObject);
                tentacles[i] = null;
                tentaclesRemaining--;
                break;
            }
        }
    }

    void BossDefeated()
    {
        // Reset combo saat boss selesai
        ComboManager.Instance.ResetCombo();

        if (bossAnimator != null)
            bossAnimator.SetTrigger("Die");

        if (bossExplosionPrefab != null)
            Instantiate(bossExplosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, destroyDelay);
    }
}
