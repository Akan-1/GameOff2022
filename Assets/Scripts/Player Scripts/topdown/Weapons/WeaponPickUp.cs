﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] private WeaponInfo weaponInfo;

    private void Start()
    {
        weaponInfo.SetAmmoAmount();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController2d player))
        {

            if (Input.GetMouseButton(1))
            {
                if (player.enabled)
                {
                    player.currentWeapon = weaponInfo;
                    player.WeaponInHand(true);

                    //GetComponent<BoxCollider2D>()= false;

                    transform.position = player.gunHolder.position;

                    transform.localRotation = Quaternion.identity;

                    weaponInfo.firePoint = player.firePointPlayer;
                    weaponInfo.weaponBody = player.transform;
                    transform.SetParent(player.transform);

                    //gameObject.SetActive(false);

                    //Это позже, когда будут спрайты в игре
                    //player.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = weaponInfo.weaponSprite;
                }
            }
        }
    }

}