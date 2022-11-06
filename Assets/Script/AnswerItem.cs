using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class AnswerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI content;

    private string remoteUserHexColor = "#4d7796";
    private string localUserHexColor = "#e1e85f";
    
    public void SetUp(string username, string answer)
    {
        string nameColor = "";
        if (PhotonNetwork.LocalPlayer.NickName == username)
            nameColor = localUserHexColor;
        else
            nameColor = remoteUserHexColor;
        
        content.text = $"<color={nameColor}>{username} :</color> {answer}";
    }

    public void ShowNotification(string notification)
    {
        content.text = $"<color=#4fdb70>{notification}</color>";
    }
}
