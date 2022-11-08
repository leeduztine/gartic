using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
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
        avt.sprite = Utility.Instance.GetAvatarById((int)player.CustomProperties["avatar"]);
        username.text = player.NickName;
        score.text = player.CustomProperties["score"].ToString();
    }
}
