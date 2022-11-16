using UnityEngine;

public class PlayerController2d : MonoBehaviour, ITakeDamage
{
    private Rigidbody2D _rb;
    private Animator _anim;
    private Transform _currentWall;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pushOffWallSound;

    private float _nextTimeOfFire = 0;

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

    public Transform CurrentWall
    {
        get => _currentWall;
        private set
        {
            _currentWall = value;
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
    public Transform firePointPlayer;

    [Space]
    [Header("Squat Config")]
    [SerializeField] private LayerMask _roofMask;
    [SerializeField] private Transform _topCheck;
    [SerializeField] private float _topCheckRadius;
    [SerializeField] private Collider2D _poseStand;
    [SerializeField] private Collider2D _poseSquat;

    public GunHolder GunHolder => _gunHolder;
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
        CheckMovement();
        CheckMovementDirection();
        UpdateAnimation();
        CheckSurroundings();
        WallSlide();
        CanJump();

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
        if (_currentWall != null && _isTouchWall && !_isGround && _rb.velocity.y < 0 && isHasWallJumps)
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
                _rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                _isWallSliding = false;
                _movementInputDirection = -_facingDirection;
            }

            if ((_isWallSliding || _currentWall != null) && _movementInputDirection != 0)
            {
                Vector2 forceToAdd = new Vector2(_wallJumpDirection.x * _wallJumpForce * _movementInputDirection, _wallJumpDirection.y * _wallSpeed);
                _rb.AddForce(forceToAdd, ForceMode2D.Impulse);
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
        _isGround = Physics2D.OverlapCircle(_groudCheck.position, _groundCheckRadius, _whatIsGround);
        RaycastHit2D hit = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckRadius, _wallMask);

        if (hit)
        {

            _isTouchWall = true;
            if (CurrentWall != hit.collider?.transform)
            {
                CurrentWall = hit.collider?.transform;
            }
        } else
        {
            _isTouchWall = false;
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
