using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerActivityTrigger : MonoBehaviour
{
    private BoxCollider2D _boxCollider;
    private bool _isDisactivated = true;

    [SerializeField] private UnityEvent _onEnter;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDisactivated && collision.TryGetComponent(out PlayerController2d _playerController2D))
        {
            _onEnter.Invoke();
            _isDisactivated = false;
            gameObject.SetActive(false);
        }
    }
}
