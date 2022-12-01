using QFSW.MOP2;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField] private ObjectPoolsNames _poolName;

    [SerializeField] private int damage = 1;
    [SerializeField] float bulletLifetime = 3f;

    private void OnEnable()
    {
        Invoke(nameof(ReleaseBullet), bulletLifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(damage);
        }

        ReleaseBullet();
    }

    private void ReleaseBullet()
    {
        if (gameObject.activeInHierarchy)
        {
            MasterObjectPooler.Instance.Release(gameObject, $"{_poolName}");
        }
    }
}
