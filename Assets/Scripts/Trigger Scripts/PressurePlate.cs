using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PressurePlate : MonoBehaviour
{
    private Animator animator;

    public int pressureIndex;
    [Space]
    [SerializeField] private bool _isNeedHeavyBox;
    [Space]
    [SerializeField] private Collider2D _unPressed;
    [SerializeField] private Collider2D _onPressed;

    public static Action<PressurePlate> onPressed;
    public static Action onLeaved;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowPressedCollider()
    {
        _unPressed.enabled = false;
        _onPressed.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isNeedHeavyBox)
        {
            if (collision.gameObject.CompareTag("Box") || collision.gameObject.CompareTag("HeavyBox") || collision.gameObject.CompareTag("Tomas") || collision.gameObject.CompareTag("Alice"))
            {
                animator.SetBool("ButtonPressed", true);
                onPressed?.Invoke(this);
            }
        }    
        else if (_isNeedHeavyBox)
        {
            if (collision.gameObject.CompareTag("HeavyBox"))
            {
                animator.SetBool("ButtonPressed", true);
                onPressed?.Invoke(this);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetBool("ButtonPressed", false);
        _unPressed.enabled = true;
        onLeaved?.Invoke();
        Debug.Log("Leave");
    }

}
