using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI percentTxt;

    private void OnEnable()
    {
        loadingBar.value = 0f;
        loadingBar.DOValue(1f, 3f).SetEase(Ease.OutQuart);
    }

    private void Update()
    {
        percentTxt.text = $"{(int)(loadingBar.value * 100f)}%";
    }
}
