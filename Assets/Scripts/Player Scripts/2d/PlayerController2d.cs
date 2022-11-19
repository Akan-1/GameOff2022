using System;
using UnityEngine;

public class PlayerController2d : MonoBehaviour, ITakeDamage
{
    private Animator _anim;
    private Vector2 _positionOfCurrentWall;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pushOffWallSound;

    private int _facingDirection = 1;

    // переменные Bool
    private bool _isFacingRight;
    [SerializeField] private bool _canSquat;
    private bool _isGround;
    private bool _isTouchWall;
    private bool _isWallSliding;
    private bool _canJump;

    // не изменяемые переменные типа float
    private float _movementInputDirection; // хранит значение при нажатии клавиш (A, D)

    public Vector2 PositionOfCurrentWall
    {
        get => _positionOfCurrentWall;
        private set
        {
            _positionOfCurrentWall = value;
            _currentWallJumpCount = 0;
        }
    }

    // изменяемые переменные типа float
    #region Configurations
    [Space]
    [Header("Player Config")]
    [SerializeField] private Transform _groudCheck;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private int _health = 1;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    
    [Space]
    [Header("Checking wall Config")]
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private float _wallCheckRadius;
    [SerializeField] private float _wallHopForce;
    [SerializeField] private float _wallJumpForce;
    [SerializeField] private float _wallSlideSpeed;
    [SerializeField] private float _wallSpeed;
    [SerializeField] private int _maximumWallJumpCount;
    private int _currentWallJumpCount;

    [Space]
    [SerializeField] private Vector2 _wallHopDirection;
    [SerializeField] private Vector2 _wallJumpDirection;

    [Space]
    [Header("Weapon Config")]
    [SerializeField] private GunHolder _gunHolder;

    [Space]
    [Header("Squat Config")]
    [SerializeField] private LayerMask _roofMask;
    [SerializeField] private Transform _topCheck;
    [SerializeField] private float _topCheckRadius;
    [SerializeField] private Collider2D _poseStand;
    [SerializeField] private Collider2D _poseSquat;

    [Header("Animations")]
    [SerializeField] private string _idleWithRiffleAnimation = "ThomasIdleWithRifle";
    [SerializeField] private string _fallAnimation = "ThomasFall";
    public Rigidbody2D Rigibody2D
    {
        get;
        set;
    }
    public bool IsActive
    {
        get;
        set;
    }
    public GunHolder GunHolder => _gunHolder;
    #endregion
    void Awake()
    {
        Rigibody2D = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _anim.SetFloat("Speed", _speed);

        if (_canSquat)
            _topCheckRadius = _topCheck.GetComponent<CircleCollider2D>().radius;

        _wallHopDirection.Normalize();
        _wallJumpDirection.Normalize();
    }

    private void Start()
    {
        TryAddCharacterToPossible();
    }


    void Update()
    {
        if (IsActive)
        {
            CheckMovement();
            CheckMovementDirection();
            CheckSurroundings();
            WallSlide();
            CanJump();

            if (_canSquat)
                Squat();
        }
    }

    private void FixedUpdate()
    {
        ApllyMovement();
    }

    private void TryAddCharacterToPossible()
    {
        try
        {
            CharacterSwapper.Instance.AddCharacter(this);
        }
        catch (Exception e)
        {
            throw new Exception("Possible characters not found, please start game from main menu");
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        GameObjectsManager.CheckLifeAmount(_health, gameObject);
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
        if(_isGround == true)
        {
            _canJump = true;
            _currentWallJumpCount = 0;
        }
        else
        {
            _canJump = false;
        }
    }

    private void WallSlide() 
    {
        bool isHasWallJumps = _currentWallJumpCount < _maximumWallJumpCount;
        if (PositionOfCurrentWall != null && _isTouchWall && !_isGround && Rigibody2D.velocity.y < 0 && isHasWallJumps)
        {
            _movementInputDirection = 0;
            _isWallSliding = true;
            _canJump = true;
            _anim.SetBool("IsWallStick", true);
        }
        else
        {
            _isWallSliding = false;
            _canJump = false;
            _anim.SetBool("IsWallStick", false);
        }
    }

    private void CheckMovement()
    {
        if (!_isWallSliding)
        {
            _movementInputDirection = Input.GetAxisRaw("Horizontal"); // вносит значение при нажатии клавиш
            if (_movementInputDirection != 0)
            {
                _anim.SetBool("IsWalk", true);
            } else
            {
                _anim.SetBool("IsWalk", false);
            }
        }
        else
        {
            _anim.SetBool("IsWalk", false);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Physics2D.IgnoreLayerCollision(11, 15, true);
            Invoke("IgnoreLayerOff", 0.5f);
        }

        if (Input.GetButtonDown("Jump") && _isGround && _canJump)
        {
            Jump();
        }
        else if(Input.GetButton("Jump") && _isWallSliding && !_isGround)
        {
            _audioSource.PlayOneShot(_pushOffWallSound);
            JumpOnWall();
        }
    }

    private void JumpOnWall()
    {
        if (_currentWallJumpCount < _maximumWallJumpCount)
        {
            if (_isWallSliding && _movementInputDirection == 0)
            {
                Vector2 forceToAdd = new Vector2(_wallHopDirection.x * _wallHopForce * _facingDirection, _wallHopDirection.y * _wallHopForce);
                Rigibody2D.AddForce(forceToAdd, ForceMode2D.Impulse);
                _isWallSliding = false;
                _movementInputDirection = -_facingDirection;
            }

            if ((_isWallSliding || PositionOfCurrentWall != null) && _movementInputDirection != 0)
            {
                Vector2 forceToAdd = new Vector2(_wallJumpDirection.x * _wallJumpForce * _movementInputDirection, _wallJumpDirection.y * _wallSpeed);
                Rigibody2D.AddForce(forceToAdd, ForceMode2D.Impulse);
                _isWallSliding = false;
                /**/
            }
        }
        _currentWallJumpCount++;
    }

    private void ApllyMovement()
    {
        if (!_isWallSliding)
        {
            Rigibody2D.velocity = new Vector2(_movementInputDirection * _speed, Rigibody2D.velocity.y); // придаёт движение
        }

        if (_isWallSliding)
        {
            if(Rigibody2D.velocity.y < -_wallSlideSpeed)
            {
                Rigibody2D.velocity = new Vector2(Rigibody2D.velocity.x, -_wallSlideSpeed);
            }
        }
    }

    private void Jump()
    {
        Rigibody2D.velocity = new Vector2(Rigibody2D.velocity.x, _jumpForce);
    }

    private void IgnoreLayerOff() => Physics2D.IgnoreLayerCollision(11, 15, false);

    private void CheckSurroundings()
    {
        _isGround = Physics2D.OverlapCircle(_groudCheck.position, _groundCheckRadius, _whatIsGround);

        if (!_isGround)
        {
            _anim.SetBool("IsFall", true);
        } else
        {
            _anim.SetBool("IsFall", false);
        }

        RaycastHit2D hit = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckRadius, _wallMask);

        if (hit)
        {
            _isTouchWall = true;
            float roundedPositionOfCurrentWall = (float)Math.Round(PositionOfCurrentWall.x, 2);
            float roundedPositionOfHit = (float)Math.Round(hit.point.x, 2);

            bool IsNewWallPosition = roundedPositionOfCurrentWall != roundedPositionOfHit;

            if (IsNewWallPosition)
            {
                PositionOfCurrentWall = hit.point;
            }
        } else
        {
            _isTouchWall = false;
        }

    }

    private void Flip() // поворот игрока (влево, вправо)
    {
        if (!_isWallSliding)
        {
            _facingDirection *= -1;
            _isFacingRight = !_isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
/*            transform.Rotate(0, 180, 0);*/
        }      
    }

    private void Squat()
    {
        bool cantStand = !Physics2D.OverlapCircle(_topCheck.position, _topCheckRadius, _roofMask);

        if (Input.GetKey(KeyCode.LeftShift) && _isGround)
        {
            //тут для анимашки местечко, делаем параметр анимашки трушным
            _poseStand.enabled = false;
            _poseSquat.enabled = true;
            _canJump = false;
        }
        else if (cantStand)
        {
            //тут фолзим
            _poseStand.enabled = true;
            _poseSquat.enabled = false;
            _canJump = true;
        }
    }

    #region Animations

    public void UpdateWeaponAnimation()
    {
        if (GunHolder.Weapon != null)
        {
            switch (GunHolder.Weapon.WeaponInfo.Type)
            {
                case WeaponTypes.Pistol:
                    break;
                case WeaponTypes.Rifle:
                    _anim.Play(_idleWithRiffleAnimation);
                    _anim.SetBool("IsHasRifle", true);
                    break;
                case WeaponTypes.Shotgun:
                    break;

            }
        }
        else
        {
            _anim.SetBool("IsHasRifle", false);
        }
    }

    public void StopWalkAninmation()
    {
        _anim.SetBool("IsWalk", false);
    }

    public void EnableShotAnimationBool()
    {
        _anim.SetBool("IsShot", true);
    }    
    
    public void DisableShotAnimationBool()
    {
        _anim.SetBool("IsShot", false);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groudCheck.position, _groundCheckRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_wallCheck.position, _wallCheckRadius);
    }
}
