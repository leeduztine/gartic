using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Sirenix.Utilities;
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
    
    [SerializeField] private GameObject blockDrawing;
    [SerializeField] private GameObject readyPhaseGr;
    [SerializeField] private GameObject readySelect;
    [SerializeField] private GameObject readyWait;
    [SerializeField] private Image timeCounter;
    [SerializeField] private TextMeshProUGUI hintTxt;
    [SerializeField] private TMP_InputField submitInputField;
    [SerializeField] private TextMeshProUGUI answerList;
    [SerializeField] private TextMeshProUGUI revealTxt;

    [SerializeField] private LineGenerator lineGen;
    
    [SerializeField] private Button opt1;
    [SerializeField] private Button opt2;
    
    private string remoteUserHexColor = "#4d7796";
    private string localUserHexColor = "#e1e85f";

    [SerializeField] private RankItem[] rankItems;
    [SerializeField] private GameObject rank;

    [SerializeField] private GameObject replayBtn;

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
        
        if (PhotonNetwork.IsMasterClient)
            GameManager.Instance.NextTurn();
    }

    public void SetUpForDrawer()
    {
        ClearAnswerList();
        revealTxt.gameObject.SetActive(false);
        
        blockDrawing.SetActive(false);
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
                lineGen.RpcClear();
                RevealKeyWord();
                blockDrawing.SetActive(true);
                
                TimerCountDown(Config.RelaxTime, () =>
                {
                    if (!GameManager.Instance.IsFinalTurn)
                    {
                        GameManager.Instance.NextTurn();
                    }
                    else
                    {
                        Debug.LogError("end");
                        ShowRankPanel();
                    }
                });
            });
        });
    }

    public void SetUpForGuesser()
    {
        ClearAnswerList();
        revealTxt.gameObject.SetActive(false);
        
        blockDrawing.SetActive(true);
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
                lineGen.RpcClear();
                RevealKeyWord();
                blockDrawing.SetActive(true);
                
                TimerCountDown(Config.RelaxTime, () =>
                {
                    if (!GameManager.Instance.IsFinalTurn)
                    {
                        GameManager.Instance.NextTurn();
                    }
                    else
                    {
                        Debug.LogError("end");
                        ShowRankPanel();
                    }
                });
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
        ScrollToBottom();
    }

    public void SubmitAnswer()
    {
        var answer = submitInputField.text;
        submitInputField.text = "";
        
        if (!answer.IsNullOrWhitespace())
            GameManager.Instance.Submit(answer);
    }

    public void ShowWrongAnswer(string username, string answer)
    {
        PushIntoAnswerList($"  <color=#4d7796>{username} :</color> {answer}");
    }

    public void ShowCorrectAnswerNotification()
    {
        var username = PhotonNetwork.LocalPlayer.NickName;
        PushIntoAnswerList($"<color=#64d966>Congrats '{username}'! Correct answer!</color>");
    }

    public void RevealKeyWord()
    {
        revealTxt.gameObject.SetActive(true);
        revealTxt.text = $"<color=#ff0d0d>Key word: {GameManager.Instance.CurKeyWord.word.ToUpper()}</color>";
        // PushIntoAnswerList($"<color=#ff0d0d>Key word: {GameManager.Instance.CurKeyWord.word.ToUpper()}</color>");
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        answerList.transform.parent.parent.parent
            .GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
    }

    public void UpdateMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            replayBtn.SetActive(true);
        }
        else
        {
            replayBtn.SetActive(false);
        }
    }

    public void ShowRankPanel()
    {
        UpdateMasterClient();
        
        rank.SetActive(true);
        var list = GameManager.Instance.GetSortedPlayerList();
        for (int i = 0; i < 3; i++)
        {
            rankItems[i].gameObject.SetActive(false);

            if (i < list.Count)
            {
                rankItems[i].gameObject.SetActive(true);
                rankItems[i].SetUp(list[i]);
            }
        }
    }

    public void OnClickExitBtn()
    {
        GameManager.Instance.ExitMatch();
    }

    public void OnClickReplayBtn()
    {
        GameManager.Instance.ReplayMatch();
    }
}
