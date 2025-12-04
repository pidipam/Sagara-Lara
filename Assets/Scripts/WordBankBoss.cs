using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordBankBoss : MonoBehaviour

{
    private string[] words;

    void Awake()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("BankBoss");
        words = wordFile.text.Split('\n');
    }

    public string GetRandomWord()
    {
        return words[Random.Range(0, words.Length)].Trim();
    }
}
