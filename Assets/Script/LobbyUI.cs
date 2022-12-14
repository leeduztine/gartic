using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    private static LobbyUI instance;

    public static LobbyUI Instance
    {
        get => instance;
    }

    [SerializeField] private TextMeshProUGUI helloTxt;
    
    [SerializeField] private TMP_InputField createRoomId;
    [SerializeField] private TMP_InputField joinRoomId;
    
    [SerializeField] private RoomItem roomItmPrefab;
    private List<RoomItem> roomItmList = new List<RoomItem>();
    [SerializeField] private Transform roomItmContainer;

    [SerializeField] private GameObject curRoomPanel;
    [SerializeField] private TextMeshProUGUI curRoomName;
    
    [SerializeField] private PlayerItem playerItmPrefab;
    private List<PlayerItem> playerItmList = new List<PlayerItem>();
    [SerializeField] private Transform playerItmContainer;

    [SerializeField] private GameObject startBtn;
    
    // popups
    [SerializeField] private Transform createPopup;
    [SerializeField] private Transform joinPopup;
    [SerializeField] private Button openCreate;
    [SerializeField] private Button openJoin;
    [SerializeField] private Button closeCreate;
    [SerializeField] private Button closeJoin;
    
    // room options
    [SerializeField] private Button selectDefaultBtn;
    [SerializeField] private Button selectCustomBtn;
    [SerializeField] private OptionItem defaultOpt;
    [SerializeField] private OptionItem customOpt;
    [SerializeField] private Button browseFileBtn;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        ClearRoomList();
        ClearPlayerList();
        
        openCreate.onClick.RemoveAllListeners();
        openCreate.onClick.AddListener(()=>{Utility.Instance.EnablePopup(createPopup,true);});
        openJoin.onClick.RemoveAllListeners();
        openJoin.onClick.AddListener(()=>Utility.Instance.EnablePopup(joinPopup,true));
        closeCreate.onClick.RemoveAllListeners();
        closeCreate.onClick.AddListener(()=>Utility.Instance.EnablePopup(createPopup,false));
        closeJoin.onClick.RemoveAllListeners();
        closeJoin.onClick.AddListener(()=>Utility.Instance.EnablePopup(joinPopup,false));
        
        selectDefaultBtn.onClick.RemoveAllListeners();
        selectDefaultBtn.onClick.AddListener(() =>
        {
            defaultOpt.Select();
            customOpt.Unselect();
            browseFileBtn.gameObject.SetActive(false);
        });
        
        selectCustomBtn.onClick.RemoveAllListeners();
        selectCustomBtn.onClick.AddListener(() =>
        {
            customOpt.Select();
            defaultOpt.Unselect();
            browseFileBtn.gameObject.SetActive(true);
        });
        
        selectDefaultBtn.onClick.Invoke();
        
        browseFileBtn.onClick.RemoveAllListeners();
        browseFileBtn.onClick.AddListener(BrowseFile);
    }

    private void Start()
    {
        LobbyManager.Instance.JoinLobby();
    }

    public void SayHelloToPlayer(string username)
    {
        helloTxt.gameObject.SetActive(true);
        helloTxt.text = $"Hello {username}";
        helloTxt.color = new Color(1f, 1f, 1f, 0f);
        helloTxt.DOFade(1f, 1f);
    }
    
    public void EnableRoomPanel(bool isActive, string roomName = null)
    {
        if (!isActive)
        {
            curRoomPanel.SetActive(false);
            return;
        }
        
        curRoomPanel.SetActive(true);
        curRoomName.text = $"Room: {roomName}";
    }

    public void OnClickCreateBtn()
    {
        if (createRoomId.text.IsNullOrWhitespace())
        {
            GlobalUI.Instance.ShowMsgLog("Invalid Room ID. Try another!");
        }
        else
        {
            LobbyManager.Instance.CreateRoom(createRoomId.text);
        }
    }
    
    public void OnClickJoinBtn()
    {
        if (joinRoomId.text.IsNullOrWhitespace())
        {
            GlobalUI.Instance.ShowMsgLog("Invalid Room ID. Try another!");
        }
        else
        {
            LobbyManager.Instance.JoinRoom(joinRoomId.text);
        }
    }

    public void OnClickBackBtn()
    {
        LobbyManager.Instance.Disconnect();
    }

    public void OnClickLeaveBtn()
    {
        LobbyManager.Instance.LeaveRoom();
    }

    public void OnClickStartBtn()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        roomList.ForEach(room =>
        {
            int i = roomItmList.FindIndex(r => r.name == room.Name);
            if (room.RemovedFromList && i != -1)
            {
                Destroy(roomItmList[i].gameObject);
                roomItmList.RemoveAt(i);
            }
            if (!room.RemovedFromList && i == -1)
            {
                RoomItem newRoom = Instantiate(roomItmPrefab, roomItmContainer);
                if (newRoom)
                {
                    newRoom.SetUp(room.Name);
                    newRoom.name = room.Name;
                    roomItmList.Add(newRoom);
                }
            }
        });
    }

    public void UpdatePlayerList()
    {
        ClearPlayerList();

        if (PhotonNetwork.CurrentRoom == null) return;

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayer = Instantiate(playerItmPrefab, playerItmContainer);
            playerItmList.Add(newPlayer);
            newPlayer.name = player.Value.NickName;
            newPlayer.SetUp(player.Value);
            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayer.ApplyLocalChanges();
            }
        }
        
        UpdateRoomMaster();
    }

    private void UpdateRoomMaster()
    {
        if (PhotonNetwork.IsMasterClient /*&& PhotonNetwork.CurrentRoom.PlayerCount > 1*/)
        {
            startBtn.SetActive(true);
        }
        else
        {
            startBtn.SetActive(false);
        }
    }

    public void ClearRoomList()
    {
        if (roomItmList != null)
            roomItmList.Clear();
        else
            roomItmList = new List<RoomItem>();
        
        foreach (Transform child in roomItmContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearPlayerList()
    {
        if (playerItmList != null)
            playerItmList.Clear();
        else
            playerItmList = new List<PlayerItem>();
        
        foreach (Transform child in playerItmContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private string path;

    public string customKeywordSet = null;

    public void BrowseFile()
    {
        string type = NativeFilePicker.ConvertExtensionToFileType("txt");
        
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((p) =>
        {
            if (p == null)
            {
                Debug.Log("Operation cancelled");
            }
            else
            {
                path = p;
                StartCoroutine(ReadFileTxt());
            }
        },new string[] {type});
    }

    IEnumerator ReadFileTxt()
    {
        if (path != null)
        {
            UnityWebRequest uwr = UnityWebRequest.Get("file:///" + path);
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var result = uwr.downloadHandler.text;
                Debug.Log(result);
                customKeywordSet = result;

                // string[] list = result.Split('\n');
                // foreach (var itm in list) Debug.Log(itm);
            }
        }
    }
}
