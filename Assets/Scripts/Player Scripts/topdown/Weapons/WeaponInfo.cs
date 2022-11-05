using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponInfo : ScriptableObject
{
    [SerializeField] private Sprite _weaponSprite;
    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float scatter;

    [SerializeField] private float _fireRate = 1;
    [SerializeField] private float bulletForce = 25;

    [HideInInspector]
    public Transform firePoint;
    [HideInInspector]
    public Transform weaponBody;

    public Sprite weaponSprite => _weaponSprite;
    public float fireRate => _fireRate;

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        Vector2 dir = weaponBody.transform.rotation * Vector2.up;
        Vector2 pdir = Vector2.Perpendicular(dir) * Random.Range(-scatter, scatter);
        rb.velocity = (dir + pdir) * bulletForce;
    }

}
