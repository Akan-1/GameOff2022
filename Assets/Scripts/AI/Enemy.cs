using System.Collections;
using System.Collections.Generic;
using QFSW.MOP2;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour, ITakeDamage
{
    public enum States
    {
        Patroling,
        Aggresive
    }

    private States _state;
    private Transform _transform;
    private float _animationCurveCurrentTime;
    private Vector3 _startScale;

    [Header("Characteristic")]
    [SerializeField] ParticlesPoolNames _bloodParticles;
    [SerializeField] private int _health;

    [Header("Movement")]
    [SerializeField] private AnimationCurve _speedIncreaseAnimationCurve;
    [SerializeField] private AnimationCurve _speedDecreaseAnimationCurve;
    [SerializeField] [Range(0.1f, 1f)] private float _distanceToStopping;
    [SerializeField] private float _agressiveAdditionalSpeed = 2;
    private AnimationCurve _speedAnimationCurve;
    private bool _isCanMove = true;
    private bool _isStopping;
    private bool _isWait;
    private IEnumerator _wait;

    [Header("StatePatroling")]
    [SerializeField] private List<Transform> _patrolingPoints = new List<Transform>();
    [SerializeField] [Min(0)] private Vector2 _stayTimeOnPointBetween;
    private int _indexOfCurrentfPatrolingPoint;
    private bool _isReversePatrooling;

    [Header("StateAggressive")]
    [SerializeField] private LayerMask _characterMask;
    [SerializeField] [Min(0)] private float _viewDistance;
    private PlayerController2d _playerController2D;

    [Header("Attack")]
    [SerializeField] private string _animationAttackName;
    [SerializeField] [Min(0)] private Vector2 _damageBetween;
    [SerializeField] [Min(0)] private float _timeBetweenAtacks = 1f;
    [SerializeField] [Min(0)] private float _distanceToAttack;
    [SerializeField] [Min(0)] private float _attackTime = .5f;
    private float _currentAtackTime;
    private bool _isAttack;
    private bool _isInZoneAttack;

    [Header("Sounds")]
    [SerializeField] private string _audioSourcePoolName = "AudioSource";
    [SerializeField] private List<AudioClip> _stepSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _attackSounds = new List<AudioClip>();
    [SerializeField] private float _stepSoundsVolume = .25f;
    [SerializeField] private float _attackSoundsVolume = .35f;
    [SerializeField] private float _audioSourceRadius = 25f;

    private Animator _animator;

    public Animator Animator
    {
        get;
        private set;
    }

    private Rigidbody2D _rigibidy2D;

    public Rigidbody2D Rigidbody2D => _rigibidy2D;
    public States State => _state;
    public bool IsCanMove => _isCanMove;

    public int Health
    {
        get => _health;
        set
        {
            if (value > 0)
            {
                _health = value;
                return;
            }

            Death();

        }
    }

    public Vector2 Target
    {
        get;
        set;
    }

    void Start()
    {
        Animator = GetComponent<Animator>();

        _rigibidy2D = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _health = Random.Range(2, 5);
        _startScale = _transform.localScale;
        GoToNextPatrolingPoint();
    }

    #region States

    public void EnablePatrolingState()
    {
        _state = States.Patroling;
    } 
    public void EnableAgressiveState()
    {
        _isStopping = false;
        _isWait = false;
        _state = States.Aggresive;
    }

    #endregion

    #region Movement

    private void MoveToTarget()
    {
        if (Target != null)
        {
            RotateToTarget();
           _rigibidy2D.velocity = GetMovementVector() * GetCurrentSpeed();


            if (_state == States.Patroling)
            {
                Patroling();
            }
            else
            {
                AggressiveToTarget();
            }
        } else
        {
            Debug.LogWarning($"Target of {gameObject.name} is null");
        }
    }

    public void Patroling()
    {
        if (GetDistanceToTarget() <= _distanceToStopping)
        {
            WaitAndGoToNextPoint();
        }
    }

    public void AggressiveToTarget()
    {
        if (GetDistanceToTarget() <= _distanceToAttack)
        {

            if (_playerController2D != null)
            {
                _currentAtackTime += Time.deltaTime;
                _isInZoneAttack = true;

                if (_currentAtackTime >= _timeBetweenAtacks && !_isAttack)
                {
                    Attack();
                }
            } else
            {
                WaitAndGoToNextPoint();
                EnablePatrolingState();
            }
        }
        else
        {
            _isInZoneAttack = false;
        }
    }

    private float GetDistanceToTarget()
    {
        return Vector2.Distance(_transform.position, Target);
    }

    public Vector2 GetMovementVector()
    {
        Vector2 toTargetVector = new Vector2(Target.x - _transform.position.x, 0).normalized;
        return toTargetVector;
    }

    public float GetCurrentSpeed()
    {
        float additionSpeed = _state == States.Aggresive ? _agressiveAdditionalSpeed : 0;
        bool isNeedStoping = _isStopping || _isInZoneAttack;

        _speedAnimationCurve = isNeedStoping ? _speedDecreaseAnimationCurve : _speedIncreaseAnimationCurve;
        _animationCurveCurrentTime = isNeedStoping ? _animationCurveCurrentTime - Time.deltaTime : _animationCurveCurrentTime + Time.deltaTime;
        _animationCurveCurrentTime = Mathf.Clamp(_animationCurveCurrentTime, 0, _speedAnimationCurve.keys[_speedAnimationCurve.length - 1].time);

        float currentSpeed = _speedAnimationCurve.Evaluate(_animationCurveCurrentTime) + (isNeedStoping ? 0 : additionSpeed);
        return currentSpeed;
    }

    public void RotateToTarget()
    {
        if (Target != null)
        {
            _transform.localScale = Target.x > _transform.position.x ? _startScale : new Vector3(-_startScale.x, _startScale.y, _startScale.z);
        }
    }

    private void WaitAndGoToNextPoint()
    {
        if (!_isWait)
        {
            float waitTIme = Random.Range(_stayTimeOnPointBetween.x, _stayTimeOnPointBetween.y);
            StartWaitFor(waitTIme);
        }
    }

    public void StartWaitFor(float waitTime)
    {
        StopWait();

        _wait = WaitFor(waitTime);
        StartCoroutine(_wait);
    }

    private void StopWait()
    {
        if (_wait != null)
        {
            StopCoroutine(_wait);
        }
    }

    private IEnumerator WaitFor(float waitTime)
    {

        _isStopping = true;
        _isWait = true;

        yield return new WaitForSeconds(waitTime);

        _isStopping = false;
        _isWait = false;

        GoToNextPatrolingPoint();
    }

    #endregion

    #region Patroling

    private void GoToNextPatrolingPoint()
    {
        if (_patrolingPoints.Count > 0)
        {
            _indexOfCurrentfPatrolingPoint = GetNextIndexOfPatroolingPoint();
            Target = _patrolingPoints[_indexOfCurrentfPatrolingPoint].position;
        }
    }

    private int GetNextIndexOfPatroolingPoint()
    {
        bool isEndIndex = _indexOfCurrentfPatrolingPoint == _patrolingPoints.Count - 1;
        bool isStartIndex = _indexOfCurrentfPatrolingPoint == 0;

        if (isEndIndex || isStartIndex)
        {
            _isReversePatrooling = !_isReversePatrooling;
        }

        return _isReversePatrooling ? _indexOfCurrentfPatrolingPoint + 1: _indexOfCurrentfPatrolingPoint - 1;
    }
    #endregion

    #region Aggresive

    public void View()
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(_transform.position, _transform.right * transform.localScale.x, _viewDistance, _characterMask);
        bool isRaycastHitColliderExist = raycastHit2D.collider != null;

        Debug.Log(raycastHit2D.collider);

        if (isRaycastHitColliderExist)
        {
            bool isRaycastHitOfPlayer = raycastHit2D.collider.TryGetComponent(out PlayerController2d playerController2D);

            if (isRaycastHitOfPlayer)
            {
                StopWait();
                RunToPlayer(playerController2D);
            }
            else if (_playerController2D != null)
            {
                LoseAPlayer();
            }

            return;
        }
        else if (_playerController2D != null)
        {
            LoseAPlayer();
        }

    }

    private void RunToPlayer(PlayerController2d playerController2D)
    {
        _playerController2D = playerController2D;
        Target = _playerController2D.transform.position;
        EnableAgressiveState();
    }   
    private void LoseAPlayer()
    {
        _playerController2D = null;
        WaitAndGoToNextPoint();
        EnablePatrolingState();
    }

    private bool IsPlayerNotInViewDistance()
    {
        float distanceToPlayer = Vector2.Distance(_transform.position, _playerController2D.transform.position);
        return distanceToPlayer > _viewDistance;
    }

    #endregion

    #region Attack

    private void Attack()
    {
        if (_playerController2D != null && !_playerController2D.IsPlayerDeath)
        {
            _isAttack = true;
            _currentAtackTime = 0;
            Animator.Play(_animationAttackName);

            Invoke(nameof(StopAttack), _attackTime);
        }
    }

    public void GetDamageToPlayer()
    {
        if (_playerController2D != null && !_playerController2D.IsPlayerDeath)
        {
            int _damage = (int)Random.Range(_damageBetween.x, _damageBetween.y);
            _playerController2D.TakeDamage(_damage);
            Debug.Log($"{_playerController2D.gameObject.name} take {_damage} damage from {gameObject.name}");
        }
    }

    private void StopAttack()
    {
        _isAttack = false;
    }

    #endregion

    #region Damage

    public void TakeDamage(int damage)
    {
        ParticleCreator.Create($"{_bloodParticles}", transform.position);
        Health -= damage;
    }

    private void Death()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Audio

    public void PlayStepSound()
    {
        AudioSource audioSource = MasterObjectPooler.Instance.GetObjectComponent<AudioSource>(_audioSourcePoolName);
        audioSource.transform.position = transform.position;
        AudioPlayer.TryPlayRandom(audioSource, _stepSounds, _stepSoundsVolume, _audioSourceRadius);
    }

    public void PlayAttackSound()
    {
        AudioSource audioSource = MasterObjectPooler.Instance.GetObjectComponent<AudioSource>(_audioSourcePoolName);
        audioSource.transform.position = transform.position;
        AudioPlayer.TryPlayRandom(audioSource, _attackSounds, _attackSoundsVolume, _audioSourceRadius);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (_viewDistance) * Mathf.Clamp(transform.localScale.x, -1, 1) * transform.right);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _distanceToStopping);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + new Vector3(0, 0.2f), (_distanceToAttack / 2) * Mathf.Clamp(transform.localScale.x, -1, 1) * transform.right);
    }
}
