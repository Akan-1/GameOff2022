using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Door : MonoBehaviour
{
    [SerializeField] private int doorIndex;
    BoxCollider2D boxCollider;

    private void OnEnable()
    {
        PressurePlate.onPressed += TryOpenDoor;
        PressurePlate.onLeaved += CloseDoor;
    }

    private void OnDisable()
    {
        PressurePlate.onPressed -= TryOpenDoor;
        PressurePlate.onLeaved -= CloseDoor;
    }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void TryOpenDoor(PressurePlate plate)
    {
        if (plate.pressureIndex == doorIndex)
        {
            boxCollider.isTrigger = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 48, 86, 100);
        }
    }

    private void CloseDoor()
    {
        boxCollider.isTrigger = false;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 100, 100);
    }

}
