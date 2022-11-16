using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponInfo : ScriptableObject
{
    #region Configuration
    [Space]
    [Header("Weapon Config")]
    [SerializeField] private Sprite _weaponSprite;
    [SerializeField] private float _fireRate = 1;
    [SerializeField] private float _scatter;

    [Space]
    [Header("Bullet Config")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletForceValueOne;
    [SerializeField] private float _bulletForceValueTwo;
    [SerializeField] private int _bulletQuantityPerShoot;
    
    private float bulletForce;
    #endregion

    [HideInInspector] public Transform firePoint;
    [HideInInspector] public Transform weaponBody;

    public Sprite weaponSprite => _weaponSprite;
    public float fireRate => _fireRate;

    [SerializeField] private int _ammo;
    private int _realAmmo;

    [HideInInspector] public int ammo => _ammo;

    public void Shoot()
    {
        if (_realAmmo > 0)
        {
            for (int i = 0; i < _bulletQuantityPerShoot; i++)
            {
                GameObject bullet = Instantiate(_bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

                bulletForce = Random.Range(_bulletForceValueOne, _bulletForceValueTwo);
                rb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);

                Vector2 dir = weaponBody.transform.rotation * -Vector2.right;
                Vector2 pdir = Vector2.Perpendicular(dir) * Random.Range(-_scatter, _scatter);
                rb.velocity = (dir + pdir) * bulletForce;
            }
        }

        _realAmmo--;

        if (_realAmmo <= 0)
            _realAmmo = 0;
    }

    public void SetAmmo()
    {
        _realAmmo = _ammo;
    }
}
