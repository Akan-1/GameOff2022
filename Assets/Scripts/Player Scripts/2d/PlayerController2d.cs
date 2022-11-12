using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2d : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _anim;

    private float nextTimeOfFire = 0;
    private bool haveGun = false;


    [SerializeField] private LayerMask _whatIsGround;

    private int _facingDirection = 1;

    // переменные Bool
   [SerializeField] private bool _isFacingRight;
    private bool _isGround;
    private bool _isTouchWall;
    private bool _isWallSliding;
    private bool _canJump;

    // переменные Transform
    [SerializeField] private Transform _groudCheck;
    
    // не изменяемые переменные типа float
    private float _movementInputDirection; // хранит значение при нажатии клавиш (A, D)

    // изменяемые переменные типа float
    
    [SerializeField] private float _groundCheckRadius;
    
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    [Space]
    [Header("Checking wall configuration")]
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private float _wallCheckRadius;
    [SerializeField] private float _wallHopForce;
    [SerializeField] private float _wallJumpForce;
    [SerializeField] private float _wallSlideSpeed;
    [SerializeField] private float _wallSpeed;

    [Space]
    [SerializeField] private Vector2 _wallHopDirection;
    [SerializeField] private Vector2 _wallJumpDirection;

    [Space]
    [Header("Weapon Config")]
    public WeaponInfo currentWeapon;
    public Transform firePointPlayer;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _wallHopDirection.Normalize();
        _wallJumpDirection.Normalize();
    }
    void Update()
    {
        if (haveGun)
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time >= nextTimeOfFire)
                {
                    currentWeapon.Shoot();
                    nextTimeOfFire = Time.time + 1 / currentWeapon.fireRate;
                }
            }
        }
            CheckMovement();
        CheckMovementDirection();
        UpdateAnimation();
        CheckSurroundings();
        WallSlide();
        CanJump();
    }

    private void FixedUpdate()
    {
        ApllyMovement();
    }

    public bool WeaponInHand(bool inHand)
    {
        haveGun = inHand;
        return true;
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

    private void CanJump()
    {
        if(_isGround == true || _isTouchWall)
        {
            _canJump = true;
        }
        else
        {
            _canJump = false;
        }
    }

    private void WallSlide() 
    {
        if(_isTouchWall && !_isGround && _rb.velocity.y < 0)
        {
            _movementInputDirection = 0;
            _isWallSliding = true;
            _canJump = true;
        }
        else
        {
            _isWallSliding = false;
            _canJump = false;
        }
    }

    private void CheckMovement()
    {
        if (!_isWallSliding)
        {
            _movementInputDirection = Input.GetAxisRaw("Horizontal"); // вносит значение при нажатии клавиш
        }


        if (Input.GetButton("Jump") && _isGround && _canJump)
        {
            Jump();
        }
        else if(Input.GetButton("Jump") && _isWallSliding && _canJump)
        {
            JumpOnWall();
        }
    }

    private void JumpOnWall()
    {

        if (_isWallSliding && _movementInputDirection == 0)
        {
            Vector2 forceToAdd = new Vector2(_wallHopDirection.x * _wallHopForce * _facingDirection, _wallHopDirection.y * _wallHopForce);
            _rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            _isWallSliding = false;
            _movementInputDirection = -_facingDirection;
        }

        if ((_isWallSliding || _isTouchWall) && _movementInputDirection != 0)
        {
                Vector2 forceToAdd = new Vector2(_wallJumpDirection.x * _wallJumpForce * _movementInputDirection, _wallJumpDirection.y * _wallSpeed);
                _rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                _isWallSliding = false;
            /**/
        }
    }

    private void ApllyMovement()
    {
        if (!_isWallSliding)
        {
            _rb.velocity = new Vector2(_movementInputDirection * _speed, _rb.velocity.y); // придаёт движение
        }

        if (_isWallSliding)
        {
            if(_rb.velocity.y < -_wallSlideSpeed)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, -_wallSlideSpeed);
            }
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
    }

    private void CheckSurroundings()
    {
        _isGround = Physics2D.OverlapCircle (_groudCheck.position, _groundCheckRadius, _whatIsGround);

        _isTouchWall = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckRadius, _whatIsGround);
    }

    private void UpdateAnimation() // здесь находится вся анимация
    {

    }

    private void Flip() // поворот игрока (влево, вправо)
    {
        if (!_isWallSliding)
        {
            _facingDirection *= -1;
            _isFacingRight = !_isFacingRight;
            transform.Rotate(0, 180, 0);
        }      
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groudCheck.position, _groundCheckRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_wallCheck.position, _wallCheckRadius);
    }
}
