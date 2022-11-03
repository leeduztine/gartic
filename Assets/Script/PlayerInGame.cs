using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerInGame : MonoBehaviourPunCallbacks
{
    private Player player;

    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI score;

    [SerializeField] private Image bg;
    [SerializeField] private Color highLightColor;

    private Hashtable properties = new Hashtable();

    private bool isMyTurn = false;
    [SerializeField] private GameObject turn;

    public Player GetPlayerInfo()
    {
        return player;
    }

    public void SetUp(Player targetPlayer, bool isLocalPlayer)
    {
        player = targetPlayer;
        playerName.text = player.NickName;
        UpdatePlayerInGame(player);

        if (isLocalPlayer)
        {
            bg.color = highLightColor;
        }
        else
        {
            bg.color = Color.white;
        }
    }

    private void UpdatePlayerInGame(Player targetPlayer)
    {
        if (targetPlayer.CustomProperties.ContainsKey("avatar"))
        {
            img.sprite = Utility.Instance.GetAvatarById((int)targetPlayer.CustomProperties["avatar"]);
            properties["avatar"] = (int)targetPlayer.CustomProperties["avatar"];
        }
        else
        {
            img.sprite = Utility.Instance.GetAvatarById(0);
            properties["avatar"] = 0;
        }
        
        if (targetPlayer.CustomProperties.ContainsKey("score"))
        {
            score.text = targetPlayer.CustomProperties["score"].ToString();
            properties["score"] = (int)targetPlayer.CustomProperties["score"];
        }
        else
        {
            score.text = "0";
            properties["score"] = 0;
        }
    }

    public void StartTurn()
    {
        isMyTurn = true;
        turn.SetActive(true);
    }

    public void EndTurn()
    {
        isMyTurn = false;
        turn.SetActive(false);
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable hashtable)
    {
        Debug.Log($"properties is updated on '{targetPlayer.NickName}'");
        UpdatePlayerInGame(targetPlayer);
        GameManager.Instance.UpdatePlayerList();
    }
}
