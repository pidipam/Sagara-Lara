using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public void ShowGameOver()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;  // pause game
    }
}
