using UnityEngine;

public class PlayerController2d : MonoBehaviour, ITakeDamage
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pushOffWallSound;

    private Rigidbody2D _rb;
    private Animator _anim;

    private float nextTimeOfFire = 0;
    private bool haveGun = false;

    private int _facingDirection = 1;

    // переменные Bool
    private bool _isFacingRight;
    [SerializeField] private bool _canSquat;
    private bool _isGround;
    private bool _isTouchWall;
    private bool _isWallSliding;
    private bool _canJump;
    private bool _canMove;

    // не изменяемые переменные типа float
    private float _movementInputDirection; // хранит значение при нажатии клавиш (A, D)

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

    [Space]
    [SerializeField] private Vector2 _wallHopDirection;
    [SerializeField] private Vector2 _wallJumpDirection;
    [SerializeField] private Vector2 _ledgePosBot;
    [SerializeField] private Vector2 _ledgePos1;
    [SerializeField] private Vector2 _ledgePos2;

    [Space]
    [Header("LedgeClimb Config")]
    [SerializeField] private Transform _ledgeClimbCheck;

    [SerializeField] private float _ledgeRadius;
    [SerializeField] private float _ledgeClimbXOffset1 = 0f;
    [SerializeField] private float _ledgeClimbXOffset2 = 0f;
    [SerializeField] private float _ledgeClimbYOffset1 = 0f;
    [SerializeField] private float _ledgeClimbYOffset2 = 0f;

    private bool _isTouchingLedge;
    private bool _canClimbLedge = false;
    private bool _ledgeDetected;
    

    [Space]
    [Header("Weapon Config")]
    public WeaponInfo currentWeapon;
    public Transform firePointPlayer;
    [SerializeField] private Transform _gunHolder;

    public Transform gunHolder => _gunHolder;

    [Space]
    [Header("Squat Config")]
    [SerializeField] private LayerMask _roofMask;
    [SerializeField] private Transform _topCheck;
    [SerializeField] private float _topCheckRadius;
    [SerializeField] private Collider2D _poseStand;
    [SerializeField] private Collider2D _poseSquat;
    #endregion
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        
        if(_canSquat)
            _topCheckRadius = _topCheck.GetComponent<CircleCollider2D>().radius;

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
        CheckLedgeClimb();

        if (_canSquat)
            Squat();
    }

    private void FixedUpdate()
    {
        ApllyMovement();
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        GameObjectsManager.CheckLifeAmount(_health, gameObject);    
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
        if (!_isWallSliding || !_canMove)
        {
            _movementInputDirection = Input.GetAxisRaw("Horizontal"); // вносит значение при нажатии клавиш
        }


        if (Input.GetButton("Jump") && _isGround && _canJump)
        {
            Jump();
        }
        else if(Input.GetButton("Jump") && _isWallSliding && _canJump)
        {
            _audioSource.PlayOneShot(_pushOffWallSound);
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

    private void CheckLedgeClimb() //зацеп за края
    {
        if(_ledgeDetected && !_canClimbLedge)
        {
            _canClimbLedge = true;

            if (_isFacingRight)
            {
                _ledgePos1 = new Vector2(Mathf.Floor(_ledgePosBot.x + _wallCheckRadius) - _ledgeClimbXOffset1, Mathf.Floor(_ledgePosBot.y) + _ledgeClimbYOffset1);
                _ledgePos1 = new Vector2(Mathf.Floor(_ledgePosBot.x + _wallCheckRadius) - _ledgeClimbXOffset2, Mathf.Floor(_ledgePosBot.y) + _ledgeClimbYOffset2);
            }
            else
            {
                _ledgePos2 = new Vector2(Mathf.Ceil(_ledgePosBot.x + _wallCheckRadius) - _ledgeClimbXOffset1, Mathf.Floor(_ledgePosBot.y) + _ledgeClimbYOffset1);
                _ledgePos2 = new Vector2(Mathf.Ceil(_ledgePosBot.x + _wallCheckRadius) - _ledgeClimbXOffset2, Mathf.Floor(_ledgePosBot.y) + _ledgeClimbYOffset2);
            }

            _canMove = true;

            if (_canClimbLedge)
            {
                transform.position = _ledgePos1; // стартовая позиция
            }
        }
    }

    private void FinishedLedgeClimb() // вызывается через ivent и перемещает игрока на конечную позицию
    {
        _canClimbLedge = false;
        transform.position = _ledgePos2; // конечная позиция
        _canMove = true;
    }

    private void CheckSurroundings()
    {
        _isGround = Physics2D.OverlapCircle(_groudCheck.position, _groundCheckRadius, _whatIsGround);

        _isTouchWall = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckRadius, _wallMask);
        _isTouchingLedge = Physics2D.Raycast(_ledgeClimbCheck.position, transform.right, _ledgeRadius, _whatIsGround);

        if (_isTouchWall && !_isTouchingLedge && !_ledgeDetected)
        {
            _ledgeDetected = true;
            _ledgePosBot = _wallCheck.position;
        }
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

    private void Squat()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _isGround)
        {
            //тут для анимашки местечко, делаем параметр анимашки трушным
            _poseStand.enabled = false;
            _poseSquat.enabled = true;
            _canJump = false;
        }
        else if (!Physics2D.OverlapCircle(_topCheck.position, _topCheckRadius, _roofMask))
        {
            //тут фолзим
            _poseStand.enabled = true;
            _poseSquat.enabled = false;
            _canJump = true;
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
