using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Weapon : MonoBehaviour
{
    #region Configuration

    private Rigidbody2D _rigibody2D;
    private int _currentAmmoInMagazine;

    [SerializeField] private WeaponInfo _weaponInfo;
    public WeaponInfo WeaponInfo => _weaponInfo;
    public Rigidbody2D Rigibody2D
    {
        get => _rigibody2D;
    }
    public GunHolder GunHolder
    {
        get;
        set;
    }
    public float ShotDelayTime
    {
        get;
        private set;
    }
    public int BulletsAviable
    {
        get;
        set;
    }
    public int CurrentAmmoInMagazine
    {
        get => _currentAmmoInMagazine;
        set => _currentAmmoInMagazine = value;
    }

    #endregion

    private void Awake()
    {
        _rigibody2D = GetComponent<Rigidbody2D>();
        BulletsAviable = WeaponInfo.BulletsAviable;
        CurrentAmmoInMagazine = WeaponInfo.MaximumBulletsInMagazine;
    }

    #region PickUp

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController2d playerController2D))
        {
            PickUpTo(playerController2D);
        }
    }

    private void PickUpTo(PlayerController2d playerController2D)
    {
        if (Input.GetMouseButtonDown(1) && playerController2D.GunHolder.Weapon == null)
        {
            GunHolder = playerController2D.GunHolder;
            GunHolder.LoadWeapon(this);
        }
    }

    #endregion

    public void ShotFrom(PlayerController2d playerController2D)
    {
        if (CurrentAmmoInMagazine > 0)
        {
            ShotDelayTime -= Time.deltaTime;

            if (ShotDelayTime <= 0)
            {
                for (int bulletCount = 0; WeaponInfo.BulletPerShot > bulletCount; bulletCount++)
                {
                    Vector2 dir = playerController2D.transform.rotation * -Vector2.right;
                    Vector2 pdir = Vector2.Perpendicular(dir) * Random.Range(-WeaponInfo.Scatter, WeaponInfo.Scatter);

                    GameObject bullet = Instantiate(WeaponInfo.BulletPrefab);
                    float bulletNewXPosition = playerController2D.transform.position.x + WeaponInfo.DivineFirePoint.x * dir.x;
                    float bulletNewYPosition = playerController2D.transform.position.y + WeaponInfo.DivineFirePoint.y;
                    bullet.transform.position = new Vector3(bulletNewXPosition, bulletNewYPosition, transform.position.z);

                    Rigidbody2D bulletRigibody = bullet.GetComponent<Rigidbody2D>();
                    float bulletForce = Random.Range(WeaponInfo.BulletForceBetween.x, WeaponInfo.BulletForceBetween.y);
                    bulletRigibody.velocity = (dir + pdir) * bulletForce;
                    CurrentAmmoInMagazine--;
                }

                ShotDelayTime = WeaponInfo.SecondsBeforeNextShot;
            }
        }
        else
        {
            GunHolder.ReloadWeaponAfter(WeaponInfo.ReloadTime);
        }
    }

    public void ThrowOut(float throwForce, float throwAngularVelocity)
    {

        transform.position = GunHolder.transform.position;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 dir;

        dir.x = mousePos.x - transform.position.x;
        dir.y = mousePos.y - transform.position.y;
        dir.z = 0;

        gameObject.SetActive(true);

        Rigibody2D.AddForce(dir * throwForce, ForceMode2D.Impulse);
        Rigibody2D.angularVelocity = throwAngularVelocity;

        GunHolder.ClearWeapon();
    }

    public void ResetShotDelayTime()
    {
        ShotDelayTime = 0;
    }
}
