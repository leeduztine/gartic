using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Utility : MonoBehaviour
{
    private static Utility instance;

    public static Utility Instance
    {
        get => instance;
    }

    [TableList] 
    [SerializeField] private List<Avatar> avtList;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public Sprite GetAvatarById(int id)
    {
        Sprite sprite = null;
        avtList.ForEach(avt =>
        {
            if (avt.id == id)
                sprite = avt.sprite;
        });
        return sprite;
    }

    public int GetAvatarNum()
    {
        return avtList.Count;
    }
}

[Serializable]
public class Avatar
{
    public int id;
    public Sprite sprite;
}
