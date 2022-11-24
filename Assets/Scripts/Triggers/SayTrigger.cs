using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SayTrigger : MonoBehaviour
{
    private BoxCollider2D _boxCollider;

    [SerializeField] private List<string> _texts = new List<string>();
    private bool _isActiveted;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;
        _boxCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerSayer playerSayer))
        {
            if (!_isActiveted)
            {
                _boxCollider.enabled = false;
                playerSayer.SayFew(_texts);
                _isActiveted = true;
            }

        }
    }
}
