using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Door : MonoBehaviour
{
    BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void OpenDoor()
    {
        boxCollider.isTrigger = true;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    public void CloseDoor()
    {
        boxCollider.isTrigger = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 100, 100);
    }

}
