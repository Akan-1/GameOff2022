using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController2d : MonoBehaviour, ITakeDamage
{
    private Animator _anim;
    private float _xPositionOfCurrentWall;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _pushOffWallSound;
    [SerializeField] private NoiseMaker _noiseMaker;

    private int _facingDirection = 1;

    // переменные Bool
    private bool _isFacingRight;
    [SerializeField] private bool _canSquat;
    private bool _isOnGround;
    private bool _isTouchWall;
    private bool _isWallSliding;
    private bool _cantMove;

    [Header("Audio")]
    [SerializeField] private List<AudioClip> _stepSounds = new List<AudioClip>();
    [SerializeField] private float _stepNoiseRadius = 0.2f;
    [SerializeField] private float _stepSoundVolume = .5f;

    private float _movementInputDirection;

    // изменяемые переменные типа float
    #region Configurations
    [Space]
    [Header("Player Config")]
    [SerializeField] private Transform _groudCheck;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private int _health = 1;
    [SerializeField] private float _defaultSpeed;
    [SerializeField] private float _jumpForce;
    private float _speed;
    private bool _isCanMove = true;


    [Space]
    [Header("Checking wall Config")]
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private float _wallCheckRadius;
    [SerializeField] [Min(1)] private float _wallJumpStrength;
    [SerializeField] private float _wallSlideSpeed;
    [SerializeField] private float _wallSpeed;
    private float _xPreviousWallPosition;

    [Space]
    [SerializeField] private Vector2 _wallJumpDirection;

    [SerializeField] private Vector2 _ledgePosBot;
    [SerializeField] private Vector2 _ledgeStartPosition;
    [SerializeField] private Vector2 _ledgeEndPosition;

    [Space]
    [Header("LedgeClimb Config")]
    [SerializeField] private LayerMask _climbLayer;
    [SerializeField] private Transform _ledgeClimbCheck;
    [SerializeField] private float _ledgeRadius;
    [SerializeField] private float _ledgeClimbForce = 8;
    [SerializeField] private float _climbReloadTime = 0.5f;
    private IEnumerator _climbReload;

    private bool _isClimbReload;

    private bool _isTouchingLedge;
    private bool _canClimbLedge = false;
    private bool _ledgeDetected;

    [Space]
    [Header("Weapon Config")]
    [SerializeField] private GunHolder _gunHolder;

    [Header("Animations")]
    [SerializeField] private string _idleAnimation = "ThomasIdle";
    [SerializeField] private string _jumpAnimation = "ThomasJump";
    [SerializeField] private string _climbAnimation = "ThomasClimb";
    [SerializeField] private string _wallJumpAnimation = "ThomasWallJump";
    [SerializeField] private string _fallAnimation = "ThomasFall";
    public Rigidbody2D Rigibody2D
    {
        get;
        set;
    }
    public GunHolder GunHolder => _gunHolder;
    public Animator Animator
    {
        get => _anim;
        set => _anim = value;
    }
    public AudioSource AudioSource
    {
        get => _audioSource;
    }
    public float XPositionOfCurrentWall
    {
        get => _xPositionOfCurrentWall;
        private set
        {
            _xPositionOfCurrentWall = value;
        }
    }
    public bool IsActive
    {
        get;
        set;
    }
    public float Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            _anim.SetFloat("Speed", Speed);
        }
    }
    public float DefaultSpeed => _defaultSpeed;
    public bool IsCanShoot
    {
        get;
        private set;
    }
    public bool IsOnGround
    {
        get => _isOnGround;
        set
        {
            _isOnGround = value;

            if (value)
            {
                _xPreviousWallPosition = -999f;
            }


        }
    }

    public bool IsLockJump
    {
        get;
        set;
    }
    public bool IsCanJump
    {
        get;
        set;
    }
    public bool IsCanFlip
    {
        get;
        set;
    } = true;
    #endregion

    void Awake()
    {
        Rigibody2D = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        Speed = _defaultSpeed;


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
        }
    }

    private void FixedUpdate()
    {
        ApllyMovement();
    }

    public void LockMovement()
    {
        _cantMove = true;
        _movementInputDirection = 0;

        _anim.SetBool("IsWalk", false);
        PlayIdleAnimation();

    }

    public void UnlockMovement()
    {
        _cantMove = false;
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

    private void CheckMovementDirection()
    {
        if (IsCanFlip)
        {
            if (_isFacingRight && _movementInputDirection < 0)
            {
                Flip();
            }
            else if (!_isFacingRight && _movementInputDirection > 0)
            {
                Flip();
            }
        }
    }

    private void CanJump()
    {
        if (IsOnGround == true)
        {
            IsCanJump = true;
        }
        else
        {
            IsCanJump = false;
        }
    }

    private void WallSlide()
    {
        if (XPositionOfCurrentWall != _xPreviousWallPosition && _isTouchWall && !IsOnGround && Rigibody2D.velocity.y < 0)
        {
            _isWallSliding = true;
            IsCanJump = true;
            _anim.SetBool("IsWallStick", true);
        }
        else
        {
            _isWallSliding = false;
            IsCanJump = false;
            _anim.SetBool("IsWallStick", false);
        }
    }

    private void CheckMovement()
    {
        if (!_isWallSliding)
        {
            if (!_cantMove)
            {
                _movementInputDirection = Input.GetAxisRaw("Horizontal"); // вносит значение при нажатии клавиш
                if (_movementInputDirection != 0)
                {
                    _anim.SetBool("IsWalk", true);
                }
                else
                {
                    _anim.SetBool("IsWalk", false);
                }
            } else
            {
                Rigibody2D.velocity = new Vector2(0, Rigibody2D.velocity.y);
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

        if (Input.GetButtonDown("Jump") && IsOnGround && IsCanJump && !IsLockJump)
        {
            Jump();
        }
        else if (Input.GetButton("Jump") && _isWallSliding && !IsOnGround && !_anim.GetBool("IsClimb"))
        {
            _anim.Play(_wallJumpAnimation);
        }
    }

    private void JumpOnWall()
    {
        if (_isWallSliding)
        {
            Vector2 wallJumpVector = new Vector2((_wallJumpDirection.x * (transform.localScale.x > 0 ? -1 : 1) - transform.position.x), _wallJumpDirection.y) * _wallJumpStrength;   
            _xPreviousWallPosition = _xPositionOfCurrentWall;
            Rigibody2D.AddForce(wallJumpVector, ForceMode2D.Impulse);
            _isWallSliding = false;
            _movementInputDirection = -_facingDirection;
        }

        _anim.Play(_wallJumpAnimation);
        _audioSource.PlayOneShot(_pushOffWallSound);
    }


    private void ApllyMovement()
    {
        if (!_isWallSliding)
        {
            Rigibody2D.velocity = new Vector2(_movementInputDirection * Speed, Rigibody2D.velocity.y); // придаёт движение
        }

        if (_isWallSliding)
        {
            if(Rigibody2D.velocity.y < -_wallSlideSpeed)
            {
                Rigibody2D.velocity = new Vector2(Rigibody2D.velocity.x, -_wallSlideSpeed);
            }
        }
    }

    #region Climb
    private void CheckLedgeClimb()
    {
        if (_ledgeDetected)
        {
            if (!_canClimbLedge && !_isClimbReload)
            {
                Rigibody2D.gravityScale = 0;
                Rigibody2D.velocity = Vector2.zero;
                _cantMove = true;
                _canClimbLedge = true;
                _isClimbReload = true;
                _ledgeDetected = false;

                _anim.SetBool("IsClimb", true);
                _anim.Play(_climbAnimation);

                if (_climbReload == null)
                {
                    _climbReload = ClimbReload();
                    StartCoroutine(_climbReload);
                }
            }
        }
    }

    private IEnumerator ClimbReload()
    {
        yield return new WaitForSeconds(_climbReloadTime);
        _isClimbReload = false;
        _climbReload = null;
    }


    public void FinishLedgeClimb()
    {
        _cantMove = false;
        Rigibody2D.gravityScale = 3;
        Rigibody2D.velocity = new Vector2(Rigibody2D.velocity.x, 0);
        Rigibody2D.AddForce(Vector2.up * _ledgeClimbForce, ForceMode2D.Impulse);
        _anim.SetBool("IsClimb", false);
        _canClimbLedge = false;
    }

    #endregion
    private void Jump()
    {
        _anim.Play(_jumpAnimation);
        Rigibody2D.velocity = new Vector2(Rigibody2D.velocity.x, _jumpForce);
    }

    private void IgnoreLayerOff() => Physics2D.IgnoreLayerCollision(11, 15, false);

    private void CheckSurroundings()
    {
        IsOnGround = Physics2D.OverlapCircle(_groudCheck.position, _groundCheckRadius, _whatIsGround);

        RaycastHit2D hit = Physics2D.Raycast(_wallCheck.position, transform.right, _wallCheckRadius, _wallMask);

        if (hit)
        {
            _isTouchWall = true;
            int roundedXPositionOfHit = Mathf.FloorToInt(hit.point.x);

            bool IsNewWallPosition = XPositionOfCurrentWall != roundedXPositionOfHit;

            if (IsNewWallPosition)
            {
                XPositionOfCurrentWall = roundedXPositionOfHit;
            }

            Debug.Log($"current wall position:{XPositionOfCurrentWall}. Previous {_xPreviousWallPosition}");
        } else
        {
            _isTouchWall = false;
        }

        if (!IsOnGround)
        {
            IsCanShoot = false;
        }
        else
        {
            IsCanShoot = true;
        }

        if (!_isWallSliding && !IsOnGround)
        {
            _anim.SetBool("IsFall", true);
        }
        else
        {
            _anim.SetBool("IsFall", false);
        }

        if (!_isClimbReload && !IsOnGround)
        {
            bool _isTouchingLedge = Physics2D.OverlapCircle(_ledgeClimbCheck.transform.position, _ledgeRadius, _climbLayer);

            if (_isTouchingLedge && !_ledgeDetected)
            {
                Debug.Log(_isTouchingLedge);
                _ledgeDetected = true;
                _ledgePosBot = _wallCheck.position;
            }
        }
    }

    private void Flip() // поворот игрока (влево, вправо)
    {
        if (!_isWallSliding)
        {
            _facingDirection *= -1;
            _isFacingRight = !_isFacingRight;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
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

    public void PlayIdleAnimation()
    {
        _anim.Play(_idleAnimation);
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

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_ledgeClimbCheck.position, _ledgeRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(new Vector2(transform.position.x - (_wallJumpDirection.x * (transform.localScale.x > 0 ? -1 : 1)), transform.position.y + _wallJumpDirection.y) * _wallJumpStrength, .4f);
    }

    public void PlayStepSound()
    {
        _noiseMaker.PlayRandomAudioWithCreateNoise(_stepSounds, _stepSoundVolume, _stepNoiseRadius);
    }

    public void DisableNoise()
    {
        _noiseMaker.Noise.enabled = false;
    }
}
