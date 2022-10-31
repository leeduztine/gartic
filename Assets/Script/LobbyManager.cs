using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private static LobbyManager instance;

    public static LobbyManager Instance
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

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom(string id)
    {
        RoomOptions opt = new RoomOptions
        {
            MaxPlayers = 5,
            BroadcastPropsChangeToAll = true,
        };
        
        PhotonNetwork.CreateRoom(id);
    }

    public void JoinRoom(string id)
    {
        PhotonNetwork.JoinRoom(id);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #region Override Methods

    public override void OnCreatedRoom()
    {
        Debug.Log($"Photon:: create room '{PhotonNetwork.CurrentRoom.Name}': successful ");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Photon:: join room '{PhotonNetwork.CurrentRoom.Name}': successful ");
        LobbyUI.Instance.EnableRoomPanel(true, PhotonNetwork.CurrentRoom.Name);
        LobbyUI.Instance.UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        LobbyUI.Instance.UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LobbyUI.Instance.UpdatePlayerList();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Photon:: left room");
        LobbyUI.Instance.EnableRoomPanel(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Photon:: create room: failed \n {message}");
        GlobalUI.Instance.ShowMsgLog(message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Photon:: join room: failed \n {message}");
        GlobalUI.Instance.ShowMsgLog(message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        LobbyUI.Instance.UpdateRoomList(roomList);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Photon:: joined lobby");
        LobbyUI.Instance.SayHelloToPlayer(PhotonNetwork.NickName);
    }
    
    public override void OnLeftLobby()
    {
        Debug.Log($"Photon:: left lobby");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Photon:: rejoin lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Photon:: disconnected");
        SceneManager.LoadScene("Login");
    }

    #endregion
}
