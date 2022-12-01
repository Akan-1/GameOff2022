using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyViewZone : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D;
    [SerializeField] private Enemy _enemy;
    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _boxCollider2D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController2d playerController2D))
        {
            _enemy.Target = playerController2D.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController2d playerController2D))
        {
            _enemy.Target = Vector2.zero;
        }
    }
}
