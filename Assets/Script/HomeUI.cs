using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class HomeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private GameObject loadingScreen;

    public void OnClickConnectBtn()
    {
        var str = usernameInput.text;
        if (str.IsNullOrWhitespace())
        {
            GlobalUI.Instance.ShowMsgLog("Invalid name! Try another.");
            return;
        }

        loadingScreen.SetActive(true);
        PhotonNetwork.NickName = str;
        HomeManager.Instance.ConnectToServer();
    }
}
