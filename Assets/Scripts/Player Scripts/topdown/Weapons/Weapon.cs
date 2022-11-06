using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponInfo _weaponInfo;
    [SerializeField] private Transform _firePointWeapon;

    [SerializeField] private int _ammoValueOne;
    [SerializeField] private int _ammoValueTwo;

    public int ammo ;

    private float nextTimeOfFire = 0;

    private void Start()
    {
        ammo = Random.Range(_ammoValueOne, _ammoValueTwo);
        GetComponent<SpriteRenderer>().sprite = _weaponInfo.weaponSprite;
        _weaponInfo.firePoint = _firePointWeapon;
        _weaponInfo.weaponBody = transform;
    }

    private void Update()
    {
        if (ammo > 0)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time >= nextTimeOfFire)
                {
                    _weaponInfo.Shoot();
                    nextTimeOfFire = Time.time + 1 / _weaponInfo.fireRate;
                    ammo--;
                }
            }
        }

        if (ammo <= 0)
            ammo = 0;
    }

}
