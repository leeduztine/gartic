using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameUI : MonoBehaviour
{
    private static GameUI instance;

    public static GameUI Instance
    {
        get => instance;
    }
    
    [SerializeField] private TextMeshProUGUI introTimerTxt;
    
    [SerializeField] private Image blockDrawing;
    [SerializeField] private GameObject readyPhaseGr;
    [SerializeField] private GameObject readySelect;
    [SerializeField] private GameObject readyWait;
    [SerializeField] private Image timeCounter;
    [SerializeField] private TextMeshProUGUI hintTxt;
    [SerializeField] private TMP_InputField submitInputField;
    [SerializeField] private TextMeshProUGUI answerList;
    
    [SerializeField] private Button opt1;
    [SerializeField] private Button opt2;
    
    private string remoteUserHexColor = "#4d7796";
    private string localUserHexColor = "#e1e85f";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance!=this)
            Destroy(gameObject);
    }

    private void Start()
    {
        opt1.onClick.RemoveAllListeners();
        opt1.onClick.AddListener(() =>
        {
            GameManager.Instance.SetKeyWord(opt1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        });
        
        opt2.onClick.RemoveAllListeners();
        opt2.onClick.AddListener(() =>
        {
            GameManager.Instance.SetKeyWord(opt2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        });
    }

    public void SetUpIntro()
    {
        StartCoroutine(StartGameAfter(Config.IntroTime));
    }
    
    IEnumerator StartGameAfter(float delay)
    {
        introTimerTxt.transform.parent.gameObject.SetActive(true);
        introTimerTxt.fontSize = 400f;
        for (int i = (int)delay; i > 0; i--)
        {
            introTimerTxt.text = i.ToString();
            introTimerTxt.color = Color.white;
            Sequence seq = DOTween.Sequence();
            seq.Append(introTimerTxt.transform.DOScale(1.1f, 0.1f)).
                Append(introTimerTxt.transform.DOScale(1f,0.1f)).
                Append(introTimerTxt.DOFade(0f,0.5f));
            yield return new WaitForSeconds(1f);
        }

        introTimerTxt.fontSize = 175f;
        introTimerTxt.text = "LET'S GO";
        introTimerTxt.color = Color.yellow;
        yield return new WaitForSeconds(1f);
        introTimerTxt.transform.parent.gameObject.SetActive(false);
        
        GameManager.Instance.NextTurn();
    }

    public void SetUpForDrawer()
    {
        ClearAnswerList();
        
        blockDrawing.gameObject.SetActive(false);
        readyPhaseGr.SetActive(true);
        submitInputField.gameObject.SetActive(false);
        
        readySelect.SetActive(true);
        SetUpOptions();
        readyWait.SetActive(false);
        hintTxt.text = "nobody knows...";
        
        TimerCountDown(Config.WaitingTime, () =>
        {
            if (GameManager.Instance.CurKeyWord.word == DynamicData.nullKeyWord.word)
            {
                GenRandomKeyword();
            }
            
            readySelect.SetActive(false);
            readyPhaseGr.SetActive(false);
            submitInputField.gameObject.SetActive(true);
            
            TimerCountDown(Config.SubmitTime, () =>
            {
                if (!GameManager.Instance.IsFinalTurn)
                {
                    GameManager.Instance.NextTurn();
                }
            });
        });
    }

    public void SetUpForGuesser()
    {
        ClearAnswerList();
        
        blockDrawing.gameObject.SetActive(true);
        readyPhaseGr.SetActive(true);
        submitInputField.gameObject.SetActive(false);
        
        readySelect.SetActive(false);
        readyWait.SetActive(true);
        hintTxt.text = "nobody knows...";
        
        TimerCountDown(Config.WaitingTime, () =>
        {
            readyWait.SetActive(false);
            readyPhaseGr.SetActive(false);
            submitInputField.gameObject.SetActive(true);
            
            TimerCountDown(Config.SubmitTime, () =>
            {
                if (!GameManager.Instance.IsFinalTurn)
                {
                    GameManager.Instance.NextTurn();
                }
            });
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

    public void SetUpOptions()
    {
        string keyword1;
        string keyword2;
        DynamicData.Instance.GetRandomKeyWords(out keyword1, out keyword2);
        opt1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = keyword1;
        opt2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = keyword2;
    }

    public void GenRandomKeyword()
    {
        // if the turn owner doesnt select any opts
        // => keyword is automatically take value from one of 2 opts

        Debug.Log("Drawer select nothing!");
        int rd = Random.Range(0, 2);
        if (rd == 0) 
            opt1.onClick.Invoke();
        else 
            opt2.onClick.Invoke();
    }

    public void SetupHint()
    {
        var sizeList = GameManager.Instance.CurKeyWord.sizeList;
        hintTxt.text = "";

        for (int i = 0; i < sizeList.Count; i++)
        {
            string slots = "";
            for (int j = 0; j < sizeList[i]; j++)
            {
                slots += "_ ";
            }

            if (i < sizeList.Count - 1)
                slots += "     ";

            hintTxt.text += slots;
        }
    }

    public void ClearAnswerList()
    {
        answerList.text = $"<color=#f7d87c>Submit your answer quickly!!!</color>";
        GameManager.Instance.UpdateAnswerList(answerList.text);
    }

    public void PushIntoAnswerList(string content)
    {
        answerList.text = answerList.text + "\n" + content;
        GameManager.Instance.UpdateAnswerList(answerList.text);
    }

    public void RegenerateAnswerList(string content)
    {
        answerList.text = content;
        answerList.transform.parent.parent.parent
            .GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }

    public void SubmitAnswer()
    {
        var answer = submitInputField.text;
        var username = PhotonNetwork.LocalPlayer.NickName;
        submitInputField.text = "";
        
        PushIntoAnswerList($"<color=#4d7796>{username} :</color> {answer}");
        
        GameManager.Instance.Submit(answer);
    }

    public void ShowCorrectAnswerNotification()
    {
        var username = PhotonNetwork.LocalPlayer.NickName;
        PushIntoAnswerList($"<color=#64d966>Congrats {username}! Correct answer!</color>");
    }
}
