using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{
    // private string path;
    //
    // public void BrowseFile()
    // {
    //     path = UnityEditor.EditorUtility.OpenFilePanel("Select custom Keywords set file", "", "txt");
    //     StartCoroutine(ReadFileTxt());
    // }
    //
    // IEnumerator ReadFileTxt()
    // {
    //     if (path != null)
    //     {
    //         UnityWebRequest uwr = UnityWebRequest.Get("file:///" + path);
    //         yield return uwr.SendWebRequest();
    //
    //         if (uwr.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.Log(uwr.error);
    //         }
    //         else
    //         {
    //             Debug.Log(uwr.downloadHandler.text);
    //         }
    //     }
    // }
}
