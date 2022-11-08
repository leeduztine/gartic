using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Utility : MonoBehaviour
{
    private static Utility instance;

    public static Utility Instance
    {
        get => instance;
    }

    [TableList] 
    [SerializeField] private List<Avatar> avtList;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public Sprite GetAvatarById(int id)
    {
        Sprite sprite = null;
        avtList.ForEach(avt =>
        {
            if (avt.id == id)
                sprite = avt.sprite;
        });
        return sprite;
    }

    public int GetAvatarNum()
    {
        return avtList.Count;
    }
    
    public static int ComparePlayer(Player p1, Player p2, bool compareScore)
    {
        if (compareScore)
        {
            int score1 = 0;
            int score2 = 0;

            if (p1.CustomProperties.ContainsKey("score"))
            {
                score1 = (int)p1.CustomProperties["score"];
            }
            else
            {
                p1.SetCustomProperties(new Hashtable { { "score", 0 } });
            }

            if (p2.CustomProperties.ContainsKey("score"))
            {
                score2 = (int)p2.CustomProperties["score"];
            }
            else
            {
                p2.SetCustomProperties(new Hashtable { { "score", 0 } });
            }

            if (score1 > score2) return -1;
            if (score1 < score2) return 1;
        }

        var nameComparer = String.Compare(p1.NickName, p2.NickName, StringComparison.OrdinalIgnoreCase);

        return nameComparer switch
        {
            < 0 => -1,
            > 0 => 1,
            _ => 0
        };
    }
    
    public void EnablePopup(Transform obj, bool isActive)
    {
        if (!obj.GetComponent<CanvasGroup>())
            obj.gameObject.AddComponent<CanvasGroup>();

        var cg = obj.GetComponent<CanvasGroup>();
        
        if (isActive)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(obj.DOScale(Vector3.one * 1.2f, 0.1f))
                .Append(obj.DOScale(Vector3.one * 1f, 0.2f))
                .Insert(0f,cg.DOFade(1f,0.3f))
                .InsertCallback(0f, () =>
                {
                    obj.gameObject.SetActive(true);
                    obj.localScale = Vector3.one * 0.1f;
                    cg.alpha = 0f;
                });
        }
        else
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(obj.DOScale(Vector3.one * 1.2f, 0.1f))
                .Append(obj.DOScale(Vector3.one * 0.1f, 0.2f))
                .Insert(0f,cg.DOFade(0f,0.3f))
                .InsertCallback(0f, () =>
                {
                    obj.localScale = Vector3.one * 1f;
                    cg.alpha = 1f;
                })
                .AppendCallback(() =>
                {
                    obj.gameObject.SetActive(true);
                });
        }
    }
}

[Serializable]
public class Avatar
{
    public int id;
    public Sprite sprite;
}
