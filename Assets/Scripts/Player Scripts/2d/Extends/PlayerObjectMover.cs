using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController2d))]
public class PlayerObjectMover : MonoBehaviour
{
    private PlayerController2d playerController2D;

    private void Start()
    {
        playerController2D = GetComponent<PlayerController2d>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out HeavyProp hevyProp))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                hevyProp.ConnectToPlayerController(playerController2D);
            } else if (Input.GetKeyUp(KeyCode.F))
            {
                hevyProp.Unconnect();
            }
        }
    }
}
