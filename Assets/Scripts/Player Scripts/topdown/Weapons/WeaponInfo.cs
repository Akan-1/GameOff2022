using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponInfo : ScriptableObject
{
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

    [HideInInspector]public Transform firePoint;
    [HideInInspector] public Transform weaponBody;
    
    public Sprite weaponSprite => _weaponSprite;
    public float fireRate => _fireRate;

    public void Shoot()
    {
            for (int i = 0; i < _bulletQuantityPerShoot; i++)
            {
                GameObject bullet = Instantiate(_bulletPrefab, firePoint.transform.position, Quaternion.identity);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

                bulletForce = Random.Range(_bulletForceValueOne, _bulletForceValueTwo);
                rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

                Vector2 dir = weaponBody.transform.rotation * Vector2.up;
                Vector2 pdir = Vector2.Perpendicular(dir) * Random.Range(-_scatter, _scatter);
                rb.velocity = (dir + pdir) * bulletForce;
            }
    }
}
