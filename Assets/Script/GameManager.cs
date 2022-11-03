using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get => instance;
    }

    [SerializeField] private Transform playerInGameContainer;
    private List<PlayerInGame> playerItmList = new List<PlayerInGame>();
    private List<Player> turnQueue = new List<Player>();
    private int curTurn = -1;
    private int lastTurn = 0;

    private string curKeyWord;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        foreach (Transform child in playerInGameContainer)
        {
            playerItmList.Add(child.GetComponent<PlayerInGame>());
        }
        
        UpdatePlayerList();
    }

    [Button]
    public void UpdatePlayerList()
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

    public void NextTurn()
    {
        curTurn++;
        if (curTurn > lastTurn) curTurn = 0;
        
        playerItmList.ForEach(x=>x.EndTurn());
        playerItmList.Find(x => x.GetPlayerInfo() == turnQueue[curTurn])
            .StartTurn();

        if (turnQueue[curTurn].IsLocal)
        {
            GameUI.Instance.SetUpForDrawer();
        }
        else
        {
            GameUI.Instance.SetUpForGuesser();
        }
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

    
}
