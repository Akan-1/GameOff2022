﻿using QFSW.MOP2;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Weapon : MonoBehaviour
{
    #region Configuration

    private Rigidbody2D _rigibody2D;
    private int _currentAmmoInMagazine;

    [SerializeField] private WeaponInfo _weaponInfo;
    private CircleCollider2D _pickUpTrigger;

    [Header("Audio")]
    [SerializeField] private List<AudioClip> _shotSounds;

    [Header("Particles")]
    [SerializeField] private ParticlesPoolNames _shotParticles;

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
        _pickUpTrigger = GetComponent<CircleCollider2D>();

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
        bool playerHasntWeaapon = playerController2D.GunHolder.Weapon == null;
        bool playerIsActive = playerController2D.IsActive;

        if (Input.GetMouseButtonDown(1) && playerHasntWeaapon && playerIsActive)
        {
            GunHolder = playerController2D.GunHolder;
            GunHolder.LoadWeapon(this);
            _pickUpTrigger.enabled = false;
        }
    }

    #endregion

    #region Shooting

    public void ShotFrom(PlayerController2d playerController2D)
    {
        if (CurrentAmmoInMagazine > 0)
        {
            ShotDelayTime -= Time.deltaTime;

            if (ShotDelayTime <= 0)
            {
                Vector2 playerDirection = playerController2D.transform.rotation * -Vector2.right;
                Vector2 perpendiculaarPlayerDirection = Vector2.Perpendicular(playerDirection) * Random.Range(-WeaponInfo.Scatter, WeaponInfo.Scatter);

                for (int bulletCount = 0; WeaponInfo.BulletPerShot > bulletCount; bulletCount++)
                {
                    GameObject bullet = CreateBullet();
                    SetPositionToBullet(bullet, GunHolder.transform.position, playerDirection.x);
                    AddForceToBullet(bullet, playerDirection, perpendiculaarPlayerDirection);
                }
                CurrentAmmoInMagazine--;
                ShotDelayTime = WeaponInfo.SecondsBeforeNextShot;
                GunHolder.AddRecoil(_weaponInfo.RecoilStrength);
                CreateShotParticles(GunHolder.transform.position, playerDirection.x, playerDirection.x < 0);
                CreateNoise();
            }
        }
        else
        {
            GunHolder.ReloadWeaponAfter(WeaponInfo.ReloadTime);
        }
    }

    private GameObject CreateBullet()
    {
        return MasterObjectPooler.Instance.GetObject($"{WeaponInfo.BulletPoolName}");
    }

    private void SetPositionToBullet(GameObject bullet, Vector3 startPosition, float playerDirectionX)
    {
        float bulletNewXPosition = startPosition.x + WeaponInfo.OffsetFirePoint.x * playerDirectionX;
        float bulletNewYPosition = startPosition.y + WeaponInfo.OffsetFirePoint.y;
        bullet.transform.position = new Vector3(bulletNewXPosition, bulletNewYPosition, transform.position.z);
    }

    private void AddForceToBullet(GameObject bullet, Vector2 playerDirection, Vector2 perpendicularPlayerDirection)
    {
        Rigidbody2D bulletRigibody = bullet.GetComponent<Rigidbody2D>();
        float bulletForce = Random.Range(WeaponInfo.BulletForceBetween.x, WeaponInfo.BulletForceBetween.y);
        bulletRigibody.velocity = (playerDirection + perpendicularPlayerDirection) * bulletForce;
    }

    #endregion

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
        _pickUpTrigger.enabled = true;


    }

    public void ResetShotDelayTime()
    {
        ShotDelayTime = 0;
    }

    public void CreateNoise()
    {
        GunHolder.NoiseMaker.PlayRandomAudioWithCreateNoise(_shotSounds, 1, _weaponInfo.ShotNoiseRadius);
        GunHolder.StartNoiseDisabler();
    }

    #region particles

    private void CreateShotParticles(Vector3 startPosition, float playerDirectionX ,  bool mirror)
    {
        if ($"{_shotParticles}" != "")
        {
            float particlesNewXPosition = startPosition.x + WeaponInfo.OffsetFirePoint.x * playerDirectionX;
            float particlesNewYPosition = startPosition.y + WeaponInfo.OffsetFirePoint.y;
            Vector3 newParticlesPosition = new Vector3(particlesNewXPosition, particlesNewYPosition, transform.position.z);
            ParticleCreator.Create($"{_shotParticles}".Replace(" ", ""), newParticlesPosition, mirror);
        }
    }

    #endregion
}
