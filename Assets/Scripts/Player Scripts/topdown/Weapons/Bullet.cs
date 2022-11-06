using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[SerializeField] private float damage = 1f;
    [SerializeField] float bulletLifetime = 3f;

    private void Start()
    {
        Invoke("DestroyBullet", bulletLifetime);
       // Physics2D.IgnoreLayerCollision(layer1: 9, layer2: 9);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private void DestroyBullet()
    {
        Destroy(gameObject, 1f);
    }
}
