using UnityEngine;
using System;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    private Animator animator;

    [Space]
    [SerializeField] private bool _isNeedHeavyBox;

    [SerializeField] private UnityEvent _onPressed;
    [SerializeField] private UnityEvent _onLeaved;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isNeedHeavyBox)
        {
            bool isCharacter = collision.collider.TryGetComponent(out PlayerController2d playerController2D);
            bool isHeavyBox = collision.collider.TryGetComponent(out HeavyProp heavyProp);

            if (collision.gameObject.CompareTag("Box") || heavyProp || isCharacter)
            {
                animator.SetBool("ButtonPressed", true);
                _onPressed.Invoke();
            }
        }    
        else if (_isNeedHeavyBox)
        {
            if (collision.gameObject.CompareTag("HeavyBox"))
            {
                animator.SetBool("ButtonPressed", true);
                _onPressed.Invoke();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetBool("ButtonPressed", false);
        _onLeaved?.Invoke();
    }

}
