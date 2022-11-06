using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
    public TMP_InputField terminal;
    public TextMeshProUGUI output;

    public void ExecuteCommand()
    {
        string cm = terminal.text;
        string[] list = cm.Split(' ');
        if (list[0] == "buff")
        {
            GameManager.Instance.BuffScore(list[1], int.Parse(list[2]));
        }

        if (list[0] == "score?")
        {
            output.text = $"player: {list[1]} score: {GameManager.Instance.GetPlayerScore(list[1])}" +
                          $"\n{output.text}";
        }

        if (list[0] == "player?")
        {
            GameManager.Instance.SyncPlayerList();
        }

        if (list[0] == "next")
        {
            GameManager.Instance.NextTurn();
        }

        if (list[0] == "key?")
        {
            output.text = $"Cur keyword: {GameManager.Instance.CurKeyWord.word}";
        }

        if (list[0] == "up")
        {
            GameManager.Instance.SubmitCorrectly();
        }
    }
}
