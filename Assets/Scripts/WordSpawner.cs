using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public WordBank wordBank;
    public float spawnInterval = 2f;
    public float xRange = 8f;
    public float ySpawn = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-xRange, xRange), ySpawn, 0);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.GetComponent<EnemyController>().SetWord(wordBank.GetRandomWord());
    }
}
