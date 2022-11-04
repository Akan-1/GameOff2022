using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2d : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;

    private bool _isFacingRight;
    private bool _isGround;

    [SerializeField] private Transform _groudCheck;

    [SerializeField] private LayerMask _whatIsGround;

    [SerializeField] private float _groundCheckRadius;
    private float _movementInputDirection; // хранит значение при нажатии клавиш (A, D)
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }
    void Update()
    {
        CheckMovement();
        CheckMovementDirection();
        UpdateAnimation();
        CheckGround();
    }

    private void FixedUpdate()
    {
        ApllyMovement();
    }


    private void CheckMovementDirection() // Проверяет поворот игрока (влево, вправо)
    {
        if(_isFacingRight && _movementInputDirection < 0)
        {
            Flip();
        }
        else if (!_isFacingRight && _movementInputDirection > 0)
        {
            Flip();
        }
    }

    private void CheckMovement()
    {
        _movementInputDirection = Input.GetAxisRaw("Horizontal"); // вносит значение при нажатии клавиш

        if (Input.GetButton("Jump") && _isGround)
        {
            Jump();
        }
    }

    private void ApllyMovement()
    {
        _rb.velocity = new Vector2(_movementInputDirection * _speed, _rb.velocity.y); // придаёт движение
    }

    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
    }

    private void CheckGround()
    {
        _isGround = Physics2D.OverlapCircle (_groudCheck.position, _groundCheckRadius, _whatIsGround);
    }

    private void UpdateAnimation() // здесь находится вся анимация
    {

    }

    private void Flip() // поворот игрока (влево, вправо)
    {
        _isFacingRight = !_isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groudCheck.position, _groundCheckRadius);
    }
}
