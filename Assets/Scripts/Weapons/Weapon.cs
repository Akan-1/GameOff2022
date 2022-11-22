using QFSW.MOP2;
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

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _weaponInfo.Sprite;
    }

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
        if (playerController2D.IsCanShoot)
        {
            if (CurrentAmmoInMagazine > 0)
            {
                ShotDelayTime -= Time.deltaTime;

                if (ShotDelayTime <= 0)
                {
                    playerController2D.EnableShotAnimationBool();
                    for (int bulletCount = 0; WeaponInfo.BulletPerShot > bulletCount; bulletCount++)
                    {
                        GameObject bullet = CreateBullet();
                        SetPositionToBullet(bullet, GunHolder.transform.position, playerController2D.transform.localScale.x > 0);
                        CreateShotParticles(GunHolder.transform.position, playerController2D.transform.localScale);
                        AddForceToBullet(bullet, playerController2D.transform.localScale);
                    }
                    CurrentAmmoInMagazine--;
                    ShotDelayTime = WeaponInfo.SecondsBeforeNextShot;

                    playerController2D.Rigibody2D.AddForce(-Mathf.Lerp(playerController2D.transform.localScale.x, -1, 1) * transform.right, ForceMode2D.Impulse);
                    CreateNoise();
                }
            }
            else
            {
                GunHolder.ReloadWeaponAfter(WeaponInfo.ReloadTime);
            }
        }
    }

    private GameObject CreateBullet()
    {
        return MasterObjectPooler.Instance.GetObject($"{WeaponInfo.BulletPoolName}");
    }

    private void SetPositionToBullet(GameObject bullet, Vector3 startPosition, bool mirror)
    {
        float bulletNewXPosition = startPosition.x + WeaponInfo.OffsetFirePoint.x * (mirror ? -1 : 1) ;
        float bulletNewYPosition = startPosition.y + WeaponInfo.OffsetFirePoint.y;
        bullet.transform.position = new Vector3(bulletNewXPosition, bulletNewYPosition, transform.position.z);
    }

    private void AddForceToBullet(GameObject bullet, Vector3 playerScale)
    {
        Rigidbody2D bulletRigibody = bullet.GetComponent<Rigidbody2D>();
        float bulletForce = Random.Range(WeaponInfo.BulletForceBetween.x, WeaponInfo.BulletForceBetween.y);
        bulletRigibody.velocity = new Vector2((playerScale.x > 0 ? -1 : 1), Random.Range(-_weaponInfo.Scatter, _weaponInfo.Scatter)) * bulletForce;
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

    #region Particles

    private void CreateShotParticles(Vector3 startPosition, Vector3 plyerLocalScale)
    {
        if ($"{_shotParticles}" != "")
        {
            float particlesNewXPosition = startPosition.x + WeaponInfo.OffsetFirePoint.x * (plyerLocalScale.x > 0 ? -1 : 1);
            float particlesNewYPosition = startPosition.y + WeaponInfo.OffsetFirePoint.y;
            Vector3 newParticlesPosition = new Vector3(particlesNewXPosition, particlesNewYPosition, transform.position.z);
            ParticleCreator.Create($"{_shotParticles}".Replace(" ", ""), newParticlesPosition);
        }
    }

    #endregion
}
