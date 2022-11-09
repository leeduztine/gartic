using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private Transform openingScr;
    [SerializeField] private Image openingCore;
    
    private void Start()
    {
        PlayOpeningScreen();
    }

    public void OnClickConnectBtn()
    {
        var str = usernameInput.text;
        if (str.IsNullOrWhitespace())
        {
            GlobalUI.Instance.ShowMsgLog("Invalid name! Try another.");
            return;
        }

        loadingScreen.SetActive(true);
        HomeManager.Instance.ConnectToServer(str);
    }

    [Button]
    private void PlayOpeningScreen()
    {
        openingScr.gameObject.SetActive(true);
        openingScr.localScale = Vector3.one;
        openingCore.color = Color.white;
        
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1f)
            .Append(openingCore.DOFade(0f, 1f))
            .AppendInterval(0.2f)
            .Append(openingScr.DOScale(5f,0.3f))
            .AppendCallback(()=>openingScr.gameObject.SetActive(false));
    }
}
