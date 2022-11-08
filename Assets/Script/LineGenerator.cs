using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineGenerator : MonoBehaviourPun, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Line linePrefab;

    private Line activeLine;

    private Vector3 nullPoint = new Vector3(-999f, -999f, 0f);
    private Vector3 point;

    private void Start()
    {
        point = nullPoint;
    }

    private void Update()
    {
        if (activeLine)
        {
            if (point != nullPoint)
                DrawLine(point);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) 
            && Input.GetKeyDown(KeyCode.Z) 
            && GameManager.Instance.Role == PlayerRole.Drawer)
        {
            RpcUndo();
        }
    }
    
    

    public void RpcDrawLine(Vector3 mousePos)
    {
        photonView.RPC("DrawLine",RpcTarget.All,mousePos);
    }
    [PunRPC]
    public void DrawLine(Vector3 mousePos)
    {
        if (activeLine)
            activeLine.UpdateLine(mousePos);
    }
    
    
    

    public void RpcClear()
    {
        photonView.RPC("Clear",RpcTarget.All);
    }
    [PunRPC]
    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    
    

    public void RpcUndo()
    {
        photonView.RPC("Undo",RpcTarget.All);
    }
    [PunRPC]
    public void Undo()
    {
        if (transform.childCount < 1) return;
        
        Destroy(transform.GetChild(transform.childCount - 1).gameObject);
    }
    
    
    
    

    public void RpcStartLine()
    {
        photonView.RPC("StartLine",RpcTarget.All);
    }
    [PunRPC]
    public void StartLine()
    {
        Line newLine = Instantiate(linePrefab, transform);
        activeLine = newLine;
    }
    
    
    
    

    public void RpcEndLine()
    {
        photonView.RPC("EndLine",RpcTarget.All);
    }
    [PunRPC]
    public void EndLine()
    {
        activeLine = null;
        point = nullPoint;
    }
    
    
    
    

    public void RpcUpdatePoint(Vector3 p)
    {
        photonView.RPC("UpdatePoint",RpcTarget.All,p);
    }
    [PunRPC]
    public void UpdatePoint(Vector3 p)
    {
        this.point = p;
    }

    
    
    

    

    public void OnPointerDown(PointerEventData eventData)
    {
        RpcStartLine();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        RpcEndLine();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RpcUpdatePoint(mousePos);
    }
}
