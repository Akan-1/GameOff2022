using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponInfo _weaponInfo;
    [SerializeField] private Transform _firePoint;

    private float nextTimeOfFire = 0;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = _weaponInfo.weaponSprite;
        _weaponInfo.firePoint = _firePoint;
        _weaponInfo.weaponBody = this.transform;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time >= nextTimeOfFire)
                {
                    _weaponInfo.Shoot();
                    nextTimeOfFire = Time.time + 1 / _weaponInfo.fireRate;
                }
            }
        }
    }

}
