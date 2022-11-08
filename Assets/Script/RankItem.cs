using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : MonoBehaviour
{
    [SerializeField] private Image avt;
    [SerializeField] private TextMeshProUGUI username;
    [SerializeField] private TextMeshProUGUI score;

    public void SetUp(Player player)
    {
        avt.sprite = player.CustomProperties.ContainsKey("avatar") ?
            Utility.Instance.GetAvatarById((int)player.CustomProperties["avatar"])
            : Utility.Instance.GetAvatarById(0);
        
        username.text = player.NickName;
        
        score.text = player.CustomProperties.ContainsKey("score") ?
            player.CustomProperties["score"].ToString()
            : "0";
    }

    [Button]
    public void Test()
    {
        SetUp(PhotonNetwork.LocalPlayer);
    }
}
