using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionItem : MonoBehaviour
{
    [SerializeField] private bool activeOnInit = false;
    [SerializeField] private Image check;

    private bool isActive = false;

    public bool IsActive
    {
        get => isActive;
    }
    
    private void Start()
    {
        if (activeOnInit)
        {
            Select();
        }
        else
        {
            Unselect();
        }
    }

    public void Select()
    {
        isActive = true;
        check.gameObject.SetActive(true);
    }
    
    public void Unselect()
    {
        isActive = false;
        check.gameObject.SetActive(false);
    }
}
