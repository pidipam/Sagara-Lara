
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public GameObject loadingScreen;
    public Slider progressBar;

    public void Play()
    {
        StartCoroutine(LoadScene("MainScene"));
    }

    IEnumerator LoadScene(string sceneName)
    {
        // Trigger animasi fade out
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        // Tampilkan loading screen
        loadingScreen.SetActive(true);

        // Mulai load scene async
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            // Jika sudah hampir selesai (progress >= 0.9)
            if (operation.progress >= 0.9f)
            {
                // Tunggu sebentar sebelum aktifkan scene
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
