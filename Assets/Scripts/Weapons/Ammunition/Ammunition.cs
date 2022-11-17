using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour
{
    [SerializeField] private WeaponTypes _type;
    [SerializeField] private Vector2 _ammoCount;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController2d playerController2D))
        {
            bool isWeaponExist = playerController2D.GunHolder.Weapon != null;

            if (isWeaponExist)
            {
                bool isWeaponTypeEqualAmmoType = playerController2D.GunHolder.Weapon.WeaponInfo.Type == _type;

                if (isWeaponTypeEqualAmmoType)
                {
                    AddAmmoTo(playerController2D.GunHolder.Weapon);
                }
            }
        }
    }

    private void AddAmmoTo(Weapon weapon)
    {
        int ammoCount = (int)Random.Range(_ammoCount.x, _ammoCount.y);
        weapon.BulletsAviable += ammoCount;
        gameObject.SetActive(false);

        Debug.Log($"{ammoCount} bullet added to {weapon.name}. Current ammunition {weapon.BulletsAviable}");
    }
}
