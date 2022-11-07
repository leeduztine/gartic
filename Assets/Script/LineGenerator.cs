using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineGenerator : MonoBehaviourPun, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Line linePrefab;

    private Line activeLine;

    private void Update()
    {
        if (activeLine)
        {
            // DrawLine();
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RpcDrawLine(mousePos);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            // Undo();
            RpcUndo();
        }
    }

    public void RpcDrawLine(Vector3 mouserPos)
    {
        photonView.RPC("DrawLine",RpcTarget.All,mouserPos);   
    }

    public void RpcClear()
    {
        photonView.RPC("Clear",RpcTarget.All);
    }

    public void RpcUndo()
    {
        photonView.RPC("Undo",RpcTarget.All);
    }

    public void RpcStartLine()
    {
        photonView.RPC("StartLine",RpcTarget.All);
    }

    public void RpcEndLine()
    {
        photonView.RPC("EndLine",RpcTarget.All);
    }

    [PunRPC]
    public void DrawLine(Vector3 mousePos)
    {
        if (activeLine)
            activeLine.UpdateLine(mousePos);
    }
    
    [PunRPC]
    public void Clear()
    {
        return;
        
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    [PunRPC]
    public void Undo()
    {
        if (transform.childCount < 1) return;
        
        Destroy(transform.GetChild(transform.childCount - 1).gameObject);
    }

    [PunRPC]
    public void StartLine()
    {
        Line newLine = Instantiate(linePrefab, transform);
        activeLine = newLine;
    }

    [PunRPC]
    public void EndLine()
    {
        activeLine = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RpcStartLine();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        RpcEndLine();
    }
}
