using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] float bulletLifetime = 3f;

    private void Start()
    {
        Invoke("DestroyBullet", bulletLifetime);
        //Physics2D.IgnoreLayerCollision(layer1: 9, layer2: 10);
    }
        
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if(collision.gameObject.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
