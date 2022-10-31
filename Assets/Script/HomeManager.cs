using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviourPunCallbacks
{
    private static HomeManager instance;

    public static HomeManager Instance
    {
        get => instance;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void ConnectToServer(string username)
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = username;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region Override Methods

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
        Debug.Log($"Photon:: connected to master with username '{PhotonNetwork.NickName}'");
    }

    #endregion

    
}
