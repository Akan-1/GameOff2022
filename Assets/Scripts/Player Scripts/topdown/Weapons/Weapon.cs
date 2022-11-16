using QFSW.MOP2;
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

    #region Shooting

    public void ShotFrom(PlayerController2d playerController2D)
    {
        if (CurrentAmmoInMagazine > 0)
        {
            ShotDelayTime -= Time.deltaTime;

            if (ShotDelayTime <= 0)
            {
                for (int bulletCount = 0; WeaponInfo.BulletPerShot > bulletCount; bulletCount++)
                {
                    Vector2 playerDirection = playerController2D.transform.rotation * -Vector2.right;
                    Vector2 perpendiculaarPlayerDirection = Vector2.Perpendicular(playerDirection) * Random.Range(-WeaponInfo.Scatter, WeaponInfo.Scatter);

                    GameObject bullet = CreateBullet();
                    SetPositionToBullet(bullet, playerController2D.transform.position, playerDirection.x);
                    AddForceToBullet(bullet, playerDirection, perpendiculaarPlayerDirection);
                }
                CurrentAmmoInMagazine--;
                ShotDelayTime = WeaponInfo.SecondsBeforeNextShot;
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
        float bulletNewXPosition = startPosition.x + WeaponInfo.DivineFirePoint.x * playerDirectionX;
        float bulletNewYPosition = startPosition.y + WeaponInfo.DivineFirePoint.y;
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
    }

    public void ResetShotDelayTime()
    {
        ShotDelayTime = 0;
    }
}
