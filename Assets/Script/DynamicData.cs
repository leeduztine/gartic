using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

public class DynamicData : MonoBehaviour
{
    private static DynamicData instance;

    public static DynamicData Instance
    {
        get => instance;
    }

    private string[] rawKeys = new[] { "vo cuc kiem", "mu phu thuy", "giap gai", "cuong dao", "giap mau", "dong ho cat" };
    private List<KeyWordModel> keyWordList = new List<KeyWordModel>();

    public static KeyWordModel nullKeyWord = new KeyWordModel("-");

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        
        SetUpKeyWordList(rawKeys);
    }

    public void SetUpKeyWordList(string[] keys)
    {
        keyWordList.Clear();
        foreach (var key in keys)
        {
            var keyword = new KeyWordModel(key);
            keyWordList.Add(keyword);
        }
    }

    public void GetRandomKeyWords(out string keyword1, out string keyword2)
    {
        List<string> list = new List<string>();
        keyWordList.ForEach(keyword =>
        {
            if (keyword.word != GameManager.Instance.CurKeyWord.word)
                list.Add(keyword.word);
        });

        int rdIndex = Random.Range(0,list.Count);
        keyword1 = list[rdIndex];
        list.RemoveAt(rdIndex);
        rdIndex = Random.Range(0, list.Count);
        keyword2 = list[rdIndex];
    }
}

[Serializable]
public class KeyWordModel
{
    public string word;
    public List<int> sizeList;

    public KeyWordModel(string str)
    {
        word = str;
        
        Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
        word = regex.Replace(word, " ").Trim();
        
        sizeList = new List<int>();
        string[] elements = word.Split(" ");
        foreach (var itm in elements)
        {
            sizeList.Add(itm.Length);
        }
    }
}
