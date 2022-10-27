using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GlobalUI : MonoBehaviour
{
    private static GlobalUI instance;

    public static GlobalUI Instance
    {
        get => instance;
    }

    [SerializeField] private TextMeshProUGUI msgTxt;
    [SerializeField] private TextMeshProUGUI fpsTxt;
    [SerializeField] private TextMeshProUGUI pingTxt;
    private float delta;
    private float delay = 1f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    
    void Update ()
    {
        delay -= Time.deltaTime;
        
        if (delay <= 0)
        {
            delta += (Time.deltaTime - delta) * 0.1f;
            float fps = 1.0f / delta;
            fpsTxt.text = $"{(int)fps} fps";
            delay = 1f;
        }
    }

    public void ShowMsgLog(string msg)
    {
        DOTween.Kill("msg_log");
        
        msgTxt.text = msg;
        msgTxt.color = Color.white;

        Sequence seq = DOTween.Sequence();
        seq.Append(msgTxt.DOFade(1f, 2f))
            .Append(msgTxt.DOFade(0f, 1f))
            .SetId("msg_log");
    }

    [Button]
    public void MsgTest1()
    {
        ShowMsgLog("Hello");
    }

    [Button]
    public void MsgTest2()
    {
        ShowMsgLog("Goodbye");
    }
}
