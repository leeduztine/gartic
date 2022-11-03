using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private static GameUI instance;

    public static GameUI Instance
    {
        get => instance;
    }
    
    [SerializeField] private Image blockDrawing;
    [SerializeField] private GameObject readySelect;
    [SerializeField] private GameObject readyWait;
    [SerializeField] private Image timeCounter;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance!=this)
            Destroy(gameObject);
    }

    public void SetUpForDrawer()
    {
        blockDrawing.gameObject.SetActive(false);
        readySelect.SetActive(true);
        readyWait.SetActive(false);
        TimerCountDown(3f, () =>
        {
            readySelect.SetActive(false);
            TimerCountDown(10f);
        });
    }

    public void SetUpForGuesser()
    {
        blockDrawing.gameObject.SetActive(true);
        readySelect.SetActive(false);
        readyWait.SetActive(true);
        TimerCountDown(3f, () =>
        {
            readyWait.SetActive(false);
            TimerCountDown(10f);
        });
    }

    public void TimerCountDown(float duration, Action action = null)
    {
        DOTween.Kill(timeCounter);
        timeCounter.fillAmount = 1f;
        timeCounter.DOFillAmount(0f, duration)
            .OnComplete(() =>
            {
                action?.Invoke();
            });
    }
}
