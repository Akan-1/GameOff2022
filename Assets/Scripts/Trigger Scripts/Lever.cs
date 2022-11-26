using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lever : MonoBehaviour
{
    [SerializeField] private bool _IsActivated = false;
    bool _IsActive = false;

    public static Action<string> onResultingTag;
    
    public string ObjectTag
    {
        get;
        set;    
    }

    public bool LeverActivated
    {
        get;
        set;
    }

    private void Start()
    {
        LeverActivated = _IsActivated;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out PlayerController2d player))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!_IsActive)
                {
                    _IsActivated = true;
                    _IsActive = true;
                }
                else
                {
                    _IsActivated = false;
                    _IsActive = false;
                }
                SendInfoAboutPress(player.gameObject.tag);
            }
        }
    }

    private void SendInfoAboutPress(string tag)
    {
        ObjectTag = tag;
        onResultingTag?.Invoke(tag);
        Debug.Log("Method working");
    }
}
