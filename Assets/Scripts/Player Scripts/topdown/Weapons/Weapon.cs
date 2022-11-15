
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Weapon : MonoBehaviour
{
    #region Configuration

    private Rigidbody2D _rigibody2D;
    private float _currentTime;
    private int _currentAmmo;
    private int _remainingAmmo;

    [SerializeField] private WeaponInfo _weaponInfo;
    public WeaponInfo WeaponInfo => _weaponInfo;
    public Rigidbody2D Rigibody2D
    {
        get => _rigibody2D;
    }

    #endregion

    private void Awake()
    {
        _rigibody2D = GetComponent<Rigidbody2D>();
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
            playerController2D.GunHolder.LoadWeapon(this);
            playerController2D.WeaponInHand(true);
        }
    }

    #endregion

    public void ShotFrom(PlayerController2d playerController2D)
    {
        if (_currentAmmo > 0)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime >= WeaponInfo.SecondsBeforeNextShot)
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
                _currentAmmo--;

                _currentTime = 0;
            }
        }
    }

    public void Reload()
    {
        _currentAmmo = WeaponInfo.MaximumAmmo;
    }
}
