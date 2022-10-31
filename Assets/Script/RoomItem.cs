using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private Button btn;

    private void OnEnable()
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinRoom(roomName.text);
        });
    }

    public void SetUp(string room)
    {
        roomName.text = room;
    }
}
