using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordBank : MonoBehaviour
{
    private string[] words;

    void Awake()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("WordBankNormal");
        words = wordFile.text.Split('\n');
    }

    public string GetRandomWord()
    {
        return words[Random.Range(0, words.Length)].Trim();
    }
}
