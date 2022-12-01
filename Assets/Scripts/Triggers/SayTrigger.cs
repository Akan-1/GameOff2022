using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class SayTrigger : MonoBehaviour
{
    private BoxCollider2D _boxCollider;

    [SerializeField] private List<string> _texts = new List<string>();
    private bool _isActiveted;

    [Header("AdditionalActivities")]
    [SerializeField] private UnityEvent _onStartSay;
    [SerializeField] private UnityEvent _onEndSay;


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

                playerSayer.OnStartSay = _onStartSay;
                playerSayer.OnEndSay = _onEndSay;
                playerSayer.SayFew(_texts);
                _isActiveted = true;
            }

        }
    }
}
