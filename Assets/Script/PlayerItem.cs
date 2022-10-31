using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    private Player player;
    
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI playerName;

    private int curAvt = 0;
    private int maxAvt = 0;

    [SerializeField] private Image bg;
    [SerializeField] private Color highlightColor;
    [SerializeField] private GameObject arrowGr;

    private Hashtable playerProperties = new Hashtable();

    private void Awake()
    {
        maxAvt = Utility.Instance.GetAvatarNum() - 1;
    }

    public void SetUp(Player targetPlayer)
    {
        player = targetPlayer;
        playerName.text = player.NickName;
        UpdatePlayerItem(player);
    }

    private void UpdatePlayerItem(Player targetPlayer)
    {
        if (targetPlayer.CustomProperties.ContainsKey("avatar"))
        {
            img.sprite = Utility.Instance.GetAvatarById((int)targetPlayer.CustomProperties["avatar"]);
            playerProperties["avatar"] = (int)targetPlayer.CustomProperties["avatar"];
        }
        else
        {
            img.sprite = Utility.Instance.GetAvatarById(0);
            playerProperties["avatar"] = 0;
        }
    }

    public void SelectLeftAvt()
    {
        curAvt = (int)playerProperties["avatar"];
        
        curAvt--;
        if (curAvt < 0)
            curAvt = maxAvt;

        playerProperties["avatar"] = curAvt;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void SelectRightAvt()
    {
        curAvt = (int)playerProperties["avatar"];
        
        curAvt++;
        if (curAvt > maxAvt)
            curAvt = 0;
        
        playerProperties["avatar"] = curAvt;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void ApplyLocalChanges()
    {
        bg.color = highlightColor;
        arrowGr.SetActive(true);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable hashtable)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }
}
