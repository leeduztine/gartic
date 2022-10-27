using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    private static LobbyUI instance;

    public static LobbyUI Instance
    {
        get => instance;
    }

    [SerializeField] private TMP_InputField roomId;
    [SerializeField] private Button createBtn;
    [SerializeField] private RoomItem roomItmPrefab;
    [SerializeField] private Transform roomItmContainer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        foreach (Transform child in roomItmContainer)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void OnClickCreateBtn()
    {
        
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        
    }

    public void OnClickBack()
    {
        
    }

    public void OnClickLeave()
    {
        
    }

    [Button]
    public void UpdateRoomList(string[] roomList, string filter = null)
    {
        foreach (Transform child in roomItmContainer)
        {
            child.gameObject.SetActive(false);
            child.name = "_";
        }

        int count = 0;
        foreach (Transform child in roomItmContainer)
        {
            if (count >= roomList.Length) break;
            
            child.gameObject.SetActive(true);
            child.GetComponent<RoomItem>().SetUp(roomList[count]);
            child.name = roomList[count];
            count++;
        }
    }

    public void EnableCreateBtn()
    {
        createBtn.gameObject.SetActive(!roomId.text.IsNullOrWhitespace());
    }
}
