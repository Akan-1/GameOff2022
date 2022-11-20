using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2d : MonoBehaviour, ITakeDamage
{
    private Animator _anim;
    private Vector2 _positionOfCurrentWall;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pushOffWallSound;
    [SerializeField] private NoiseMaker _noiseMaker;

    private int _facingDirection = 1;

    // переменные Bool
    private bool _isFacingRight;
    [SerializeField] private bool _canSquat;
    private bool _isGround;
    private bool _isTouchWall;
    private bool _isWallSliding;
    private bool _canJump;
    private bool _canMove;

    [Header("Audio")]
    [SerializeField] private List<AudioClip> _stepSounds = new List<AudioClip>();
    [SerializeField] private float _stepNoiseRadius = 0.2f;

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

    public bool IsCanShoot
    {
        get;
        private set;
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
    [SerializeField] private GunHolder _gunHolder;

    [Space]
    [Header("Squat Config")]
    [SerializeField] private LayerMask _roofMask;
    [SerializeField] private Transform _topCheck;
    [SerializeField] private float _topCheckRadius;
    [SerializeField] private Collider2D _poseStand;
    [SerializeField] private Collider2D _poseSquat;

    [Header("Animations")]
    [SerializeField] private string _idleAnimation = "ThomasIdle";
    [SerializeField] private string _jumpAnimation = "ThomasJump";
    [SerializeField] private string _wallJumpAnimation = "ThomasWallJump";
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
            CheckLedgeClimb();

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
        if (!_isWallSliding || !_canMove)
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

            _anim.Play(_wallJumpAnimation);
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

    private void CheckLedgeClimb() //зацеп за края
    {
        if (_ledgeDetected && !_canClimbLedge)
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

    private void Jump()
    {
        _anim.Play(_jumpAnimation);
        Rigibody2D.velocity = new Vector2(Rigibody2D.velocity.x, _jumpForce);
    }

    private void IgnoreLayerOff() => Physics2D.IgnoreLayerCollision(11, 15, false);

    private void CheckSurroundings()
    {
        _isGround = Physics2D.OverlapCircle(_groudCheck.position, _groundCheckRadius, _whatIsGround);

        RaycastHit2D hit = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckRadius, _wallMask);

        if (hit)
        {
            _isTouchWall = true;
            float roundedPositionOfCurrentWall = (float)Math.Round(PositionOfCurrentWall.x, 0);
            float roundedPositionOfHit = (float)Math.Round(hit.point.x, 0);

            Debug.Log(roundedPositionOfHit);

            bool IsNewWallPosition = roundedPositionOfCurrentWall != roundedPositionOfHit;

            if (IsNewWallPosition)
            {
                PositionOfCurrentWall = hit.point;
            }
        } else
        {
            _isTouchWall = false;
        }

        if (!_isGround)
        {
            IsCanShoot = false;
        }
        else
        {
            IsCanShoot = true;
        }

        if (!_isWallSliding && !_isGround)
        {
/*            _anim.Play(_fallAnimation);*/
            _anim.SetBool("IsFall", true);
        }
        else
        {
            _anim.SetBool("IsFall", false);
        }

        _isTouchingLedge = Physics2D.Raycast(_ledgeClimbCheck.position, transform.right, _ledgeRadius, _whatIsGround);

        if (_isTouchWall && !_isTouchingLedge && !_ledgeDetected)
        {
            _ledgeDetected = true;
            _ledgePosBot = _wallCheck.position;
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
                    _anim.SetBool("IsHasPistol", true);
                    break;
                case WeaponTypes.Rifle:
                    _anim.SetBool("IsHasRifle", true);
                    break;
                case WeaponTypes.Shotgun:
                    _anim.SetBool("IsHasShotgun", true);
                    break;

            }
        }
        else
        {
            _anim.Play(_idleAnimation);
            _anim.SetBool("IsHasRifle", false);
            _anim.SetBool("IsHasPistol", false);
            _anim.SetBool("IsHasShotgun", false);
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
    #region Audio

    public void PlayStepSound()
    {
        _noiseMaker.PlayRandomAudioWithCreateNoise(_stepSounds, 1, _stepNoiseRadius);
        Invoke(nameof(DisableNoise), .1f);
    }

    public void DisableNoise()
    {
        _noiseMaker.Noise.enabled = false;
    }

    #endregion
}
