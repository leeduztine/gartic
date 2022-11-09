using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get => instance;
    }

    private bool isFinalTurn = false;

    public bool IsFinalTurn
    {
        get => isFinalTurn;
    }

    // player manager
    [SerializeField] private Transform playerInGameContainer;
    private List<PlayerInGame> playerItmList = new List<PlayerInGame>();
    private List<Player> turnQueue = new List<Player>();
    private int curTurn = -1;
    private int lastTurn = 0;

    private float timeLeft;

    private bool hasSubmitted = false;

    private KeyWordModel curKeyWord = DynamicData.nullKeyWord;

    public KeyWordModel CurKeyWord
    {
        get => curKeyWord;
    }

    private int turnCount = 0;
    private int turnLimit;

    private PlayerRole role = PlayerRole.None;

    public PlayerRole Role
    {
        get => role;
    }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        // clear player list before setup
        foreach (Transform child in playerInGameContainer)
        {
            playerItmList.Add(child.GetComponent<PlayerInGame>());
        }
        

        // setup
        SyncPlayerList();
        turnLimit = turnQueue.Count * Config.PhaseLimit;
        GameUI.Instance.SetUpIntro();

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("customKeywords"))
        {
            Debug.Log("custom keywords");
            
            string[] list = PhotonNetwork.CurrentRoom.CustomProperties["customKeywords"]
                .ToString().Split('\n');
            DynamicData.Instance.SetUpKeyWordList(list);
        }
        else
        {
            Debug.Log("default keywords");
        }
    }

    private void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }
    }


    
    
    
    
    
    
    // ------------------ turn-flow management -------------------------
    [Button]
    public void NextTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        SetKeyWord(DynamicData.nullKeyWord.word);
        
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("turn"))
        {
            curTurn = (int)PhotonNetwork.CurrentRoom.CustomProperties["turn"];
        }
        
        curTurn++;
        if (curTurn > lastTurn) curTurn = 0;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { {"turn", curTurn} });

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("turnCount"))
        {
            turnCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["turnCount"];
        }

        turnCount++;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "turnCount", turnCount } });
    }

    public void SyncTurn()
    {
        curTurn = (int)PhotonNetwork.CurrentRoom.CustomProperties["turn"];
        
        playerItmList.ForEach(x=>x.EndTurn());
        playerItmList.Find(x => x.GetPlayerInfo().NickName == turnQueue[curTurn].NickName)
            .StartTurn();

        if (turnQueue[curTurn].IsLocal)
        {
            role = PlayerRole.Drawer;
            GameUI.Instance.SetUpForDrawer();
        }
        else
        {
            role = PlayerRole.Guesser;
            GameUI.Instance.SetUpForGuesser();
        }

        hasSubmitted = false;
    }
    
    
    
    
    
    
    
    // ---------------------- player list management --------------------------
    // get sorted player list on server and setup player list UI
    [Button]
    public void SyncPlayerList()
    {
        var list = GetSortedPlayerList();
        var playerNum = list.Count;
        var slotNum = playerItmList.Count;
        for (int i = 0; i < slotNum; i++)
        {
            playerItmList[i].gameObject.SetActive(false);
            if (i >= playerNum) continue;

            playerItmList[i].gameObject.SetActive(true);
            playerItmList[i].SetUp(list[i],list[i].IsLocal);
        }

        turnQueue = GetAlphabeticalPlayerList();
        lastTurn = turnQueue.Count - 1;
    }

    private static List<Player> GetRawPlayerList()
    {
        return PhotonNetwork.CurrentRoom.Players.Select(player => player.Value).ToList();
    }

    [Button]
    public List<Player> GetAlphabeticalPlayerList()
    {
        var list = GetRawPlayerList();
        list.Sort((p1,p2)=>Utility.ComparePlayer(p1,p2,false));
        list.ForEach(x =>
        {
            // Debug.Log($"{x.NickName}");
        });
        return list;
    }

    [Button]
    public List<Player> GetSortedPlayerList()
    {
        var list = GetRawPlayerList();
        list.Sort((p1,p2)=>Utility.ComparePlayer(p1,p2,true));
        list.ForEach(x =>
        {
            var score = 0;
            if (x.CustomProperties.ContainsKey("score"))
                score = (int)x.CustomProperties["score"];
            // Debug.Log($"{x.NickName} {score}");
        });
        return list;
    }
    public Player GetPlayerHasCurTurn()
    {
        return turnQueue[curTurn];
    }

    
    
    
    
    
    
    
    
    // --------------------- game play management -------------------------------
    public void SetKeyWord(string key)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "keyword", key } });
    }

    public void SyncKeyWord()
    {
        curKeyWord = new KeyWordModel(PhotonNetwork.CurrentRoom.CustomProperties["keyword"].ToString());
        GameUI.Instance.SetupHint();
        Debug.Log($"Cur keyword: {curKeyWord.word}");
    }

    public int GetPlayerScore(string username)
    {
        var list = GetRawPlayerList();
        int score = 0;
        var targetPlayer = list.Find(x => x.NickName == username);
        if (targetPlayer == null) return -1; 
        if (targetPlayer.CustomProperties.ContainsKey("score"))
            score = (int)targetPlayer.CustomProperties["score"];
        
        return score;
    }
    
    [Button]
    public void BuffScore(string username, int score = 100)
    {
        var list = GetRawPlayerList();
        list.ForEach(x =>
        {
            if (x.NickName == username)
            {
                if (x.CustomProperties.ContainsKey("score"))
                {
                    var lastScore = (int)x.CustomProperties["score"];
                    x.SetCustomProperties(new Hashtable{{"score", lastScore + score}});
                }
                else
                {
                    x.SetCustomProperties(new Hashtable {{ "score", score }});
                }
            }
        });
    }

    public void SyncGameStatus()
    {
        Debug.Log($"turn count: {(int)PhotonNetwork.CurrentRoom.CustomProperties["turnCount"]}");
        
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["turnCount"] >= turnLimit)
        {
            isFinalTurn = true;
        }
    }

    public void SyncTimeLeft()
    {
        timeLeft = Config.WaitingTime + Config.SubmitTime;
    }

    public void SubmitCorrectly()
    {
        float scoreRate = timeLeft / Config.SubmitTime;
        BuffScore(PhotonNetwork.LocalPlayer.NickName, (int)(Config.BaseScore * scoreRate));
        hasSubmitted = true;
    }

    public void Submit(string answer)
    {
        if (answer.ToLower() == curKeyWord.word.ToLower() 
            && role != PlayerRole.Drawer
            && hasSubmitted == false)
        {
            SubmitCorrectly();
            GameUI.Instance.ShowCorrectAnswerNotification();
        }
        else
        {
            GameUI.Instance.ShowWrongAnswer(PhotonNetwork.LocalPlayer.NickName, answer);
        }
    }

    public void UpdateAnswerList(string content)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "answerList", content } });
    }

    public void SyncAnswerList()
    {
        GameUI.Instance.RegenerateAnswerList(PhotonNetwork.CurrentRoom.CustomProperties["answerList"].ToString());
    }
    
    
    
    
    
    
    
    
    
    // ------------------ room controller ---------------------------
    private void ResetRoomProperties()
    {
        PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        var list = GetRawPlayerList();
        list.ForEach(player =>
        {
            player.SetCustomProperties(new Hashtable { { "score", 0 } });
        });
    }
    
    public void ReplayMatch()
    {
        ResetRoomProperties();

        int tmp = 0;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("replay"))
        {
            tmp = (int)PhotonNetwork.CurrentRoom.CustomProperties["replay"];
        }
        tmp++;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { {"replay",tmp} });
    }

    public void ExitMatch()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    
    
    
    
    
    
    
    
    
    // ----------------------- override methods ---------------------------

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("turn"))
        {
            SyncTurn();
            SyncTimeLeft();
        }

        if (propertiesThatChanged.ContainsKey("turnCount"))
        {
            SyncGameStatus();
        }

        if (propertiesThatChanged.ContainsKey("keyword"))
        {
            SyncKeyWord();
        }

        if (propertiesThatChanged.ContainsKey("answerList"))
        {
            SyncAnswerList();
        }

        if (propertiesThatChanged.ContainsKey("replay"))
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SyncPlayerList();
        GameUI.Instance.UpdateMasterClient();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SyncPlayerList();
        GameUI.Instance.UpdateMasterClient();
    }
    
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
}

public enum PlayerRole
{
    None, Drawer, Guesser
}
