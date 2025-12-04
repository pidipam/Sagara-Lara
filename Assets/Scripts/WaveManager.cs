using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public class WaveInfo
{
    public string title;
    public string subtitle;
}

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public List<GameObject> enemyPrefabs;
    public GameObject bossPrefab;
    public WordBank normalWords;
    public WordBankBoss bossWords;
    public Transform bossSpawnPoint;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    [Header("UI Settings")]
    public ChapterDisplay chapterDisplay;
    public TextMeshProUGUI waveMessageText;
    public List<WaveInfo> waveInfos;

    [Header("Background Settings")]
    public List<GameObject> waveBackgroundObjects;

    [Header("Cutscene")]
    public CutsceneManager cutscene;

    private int waveIndex = 0;
    private List<Transform> shuffledSpawnPoints = new List<Transform>();
    private bool gameStarted = false;
    private Coroutine gameLoopCoroutine;
    private string currentBGM = "";

    void Start()
    {
        waveMessageText.gameObject.SetActive(false);
        ShuffleSpawnPoints();
        DisableAllBackgrounds();

        this.enabled = false; // WaveManager dimatikan dulu

        // Register event cutscene selesai
        cutscene.OnCutsceneFinished += OnCutsceneDone;

        // Mainkan opening dulu
        cutscene.PlayOpening();
    }

    void OnCutsceneDone()
    {
        if (!gameStarted)
        {
            gameStarted = true;
        }

        this.enabled = true;

        // Bersihkan musuh & background sebelum restart
        ClearEnemies();
        DisableAllBackgrounds();

        // Hentikan GameLoop lama kalau ada
        if (gameLoopCoroutine != null)
        {
            StopCoroutine(gameLoopCoroutine);
        }

        // Mulai ulang GameLoop
        gameLoopCoroutine = StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            for (int i = 0; i < waveInfos.Count; i++)
            {
                waveIndex = i + 1;
                if (waveIndex == 1)
                {
                    PlayBGM("OpeningTheme");
                    if (AudioManager.instance != null) AudioManager.instance.StopWhispering();
                }
                else if (waveIndex == 2 || waveIndex == 3)
                {
                    PlayBGM("TensionTheme");
                    if (AudioManager.instance != null) AudioManager.instance.StartWhispering();
                }
                else if (waveIndex >= 4)
                {
                    PlayBGM("DepressionTheme");
                    if (AudioManager.instance != null) AudioManager.instance.StartWhispering();
                }
                else if (waveIndex >= 5)
                {
                    PlayBGM("BossTheme");
                    if (AudioManager.instance != null) AudioManager.instance.StopWhispering();
                }

                SetActiveBackground(i);
                yield return StartCoroutine(chapterDisplay.ShowChapterRoutine(waveInfos[i].title, waveInfos[i].subtitle));
                yield return StartCoroutine(ShowWaveMessage($"Chapter {waveIndex}"));

                switch (waveIndex)
                {
                    case 1: yield return StartCoroutine(SpawnWave(35, 3.5f)); break;
                    case 2: yield return StartCoroutine(SpawnWave(36, 2f)); break;
                    case 3: yield return StartCoroutine(SpawnWave(32, 1.5f)); break;
                    case 4: yield return StartCoroutine(SpawnWave(30, 1f)); break;
                    case 5: yield return StartCoroutine(SpawnBossWave()); break;
                    case 6: yield return StartCoroutine(CalmWave()); break;
                }
            }

            // Setelah Wave 6 selesai → mainkan ending cutscene
            cutscene.PlayEnding();
            yield break; // hentikan GameLoop sampai cutscene selesai
        }
    }

    IEnumerator SpawnWave(int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnNormalEnemy();
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
    }

    void SpawnNormalEnemy()
    {
        int randPrefab = Random.Range(0, enemyPrefabs.Count);
        Transform spawnPoint = GetNextSpawnPoint();

        GameObject enemy = Instantiate(enemyPrefabs[randPrefab], spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<EnemyController>().SetWord(normalWords.GetRandomWord());
    }

    IEnumerator SpawnBossWave()
    {
        Debug.Log("Boss Wave!");

        GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        StartCoroutine(SpawnMinions());

        yield return new WaitUntil(() =>
            GameObject.FindGameObjectsWithTag("Boss").Length == 0
        );
    }

    IEnumerator SpawnMinions()
    {
        while (GameObject.FindGameObjectsWithTag("Boss").Length > 0)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < 3)
            {
                SpawnNormalEnemy();
            }

            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator CalmWave()
    {
        Debug.Log("Calm...");
        yield return new WaitForSeconds(8f);
    }

    IEnumerator ShowWaveMessage(string message)
    {
        waveMessageText.gameObject.SetActive(true);
        waveMessageText.text = message;

        Color c = waveMessageText.color;
        c.a = 0f;
        waveMessageText.color = c;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t);
            waveMessageText.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t);
            waveMessageText.color = c;
            yield return null;
        }

        waveMessageText.gameObject.SetActive(false);
    }

    void ShuffleSpawnPoints()
    {
        shuffledSpawnPoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            Transform temp = shuffledSpawnPoints[i];
            int randomIndex = Random.Range(i, shuffledSpawnPoints.Count);
            shuffledSpawnPoints[i] = shuffledSpawnPoints[randomIndex];
            shuffledSpawnPoints[randomIndex] = temp;
        }
    }

    Transform GetNextSpawnPoint()
    {
        if (shuffledSpawnPoints.Count == 0)
            ShuffleSpawnPoints();

        Transform point = shuffledSpawnPoints[0];
        shuffledSpawnPoints.RemoveAt(0);
        return point;
    }

    void PlayBGM(string bgm)
    {
        if (currentBGM == bgm)
            return;
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMusicWithFade(bgm, 2f);
            currentBGM = bgm;
        }
    }

    void DisableAllBackgrounds()
    {
        foreach (var bg in waveBackgroundObjects)
        {
            bg.SetActive(false);
        }
    }

    void SetActiveBackground(int index)
    {
        DisableAllBackgrounds();
        if (index >= 0 && index < waveBackgroundObjects.Count)
        {
            waveBackgroundObjects[index].SetActive(true);
        }
    }

    void ClearEnemies()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
        foreach (var boss in GameObject.FindGameObjectsWithTag("Boss"))
        {
            Destroy(boss);
        }
    }
}