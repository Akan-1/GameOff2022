using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ITakeDamage
{
    private enum States
    {
        Patroling,
        Aggresive
    }
    private States _state;
    private Transform _transform;
    private float _animationCurveCurrentTime;
    private Vector3 _startScale;

    [Header("Characteristic")]
    [SerializeField] private int _health;
    [SerializeField] private int _damage;

    [Header("Movement")]
    [SerializeField] private AnimationCurve _speedAnimationCurve;
    [SerializeField] [Range(0.1f, 0.5f)] private float _distanceToStopping;
    [SerializeField] private float _agressiveAdditionalSpeed = 2;
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
    [SerializeField] [Min(0)] private float _distanceToAttack;
    [SerializeField] private Vector2 _offsetToPlayer;
    private PlayerController2d _playerController2D;

    private Rigidbody2D _rigibidy2D;

    public int Health
    {
        get => _health;
        set
        {
            GameObjectsManager.CheckLifeAmount(_health, gameObject);
        }
    }

    public Transform Target
    {
        get;
        set;
    }

    void Start()
    {
        _rigibidy2D = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        _health = Random.Range(2, 5);
        _startScale = _transform.localScale;
        GoToNextPatrolingPoint();
    }

    void Update()
    {
        if (_isCanMove)
        {
            MoveToTarget();
        }

        View();
    }

    #region States

    private void EnablePatrolingState()
    {
        _state = States.Patroling;
    } 
    private void EnableAgressiveState()
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
                if (GetDistanceToTarget() <= _distanceToStopping)
                {
                    WaitAndGoToNextPoint();
                }
            }
            else
            {
                if (GetDistanceToTarget() <= _distanceToAttack)
                {
                    Debug.Log("Attack");
                }
            }
        }
    }

    private float GetDistanceToTarget()
    {
        return Vector2.Distance(_transform.position, Target.position);
    }

    private Vector2 GetMovementVector()
    {
        Vector2 toTargetVector = new Vector2(Target.position.x - _transform.position.x, _transform.position.y).normalized;
        return toTargetVector;
    }

    private float GetCurrentSpeed()
    {
        float additionSpeed = _state == States.Aggresive ? _agressiveAdditionalSpeed : 0;

        _animationCurveCurrentTime = _isStopping ? _animationCurveCurrentTime - Time.deltaTime : _animationCurveCurrentTime + Time.deltaTime;
        _animationCurveCurrentTime = Mathf.Clamp(_animationCurveCurrentTime, 0, _speedAnimationCurve.keys[_speedAnimationCurve.length - 1].time);

        float currentSpeed = _speedAnimationCurve.Evaluate(_animationCurveCurrentTime) + additionSpeed;
        return currentSpeed;
    }

    private void RotateToTarget()
    {
        if (Target != null)
        {
            _transform.localScale = Target.position.x > _transform.position.x ? _startScale : new Vector3(-_startScale.x, _startScale.y, _startScale.z);
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

        Debug.Log("Start Wait");
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
        _indexOfCurrentfPatrolingPoint = GetNextIndexOfPatroolingPoint();
        Target = _patrolingPoints[_indexOfCurrentfPatrolingPoint];
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

        if (isRaycastHitColliderExist)
        {
            bool isRaycastHitOfPlayer = raycastHit2D.collider.TryGetComponent(out PlayerController2d playerController2D);

            if (isRaycastHitOfPlayer)
            {
                StopWait();
                RunToPlayer(playerController2D);
            }
        }

        if (_playerController2D != null && IsPlayerNotInViewDistance())
        {
            LoseAPlayer();
        }
    }

    private void RunToPlayer(PlayerController2d playerController2D)
    {
        _playerController2D = playerController2D;
        Target = _playerController2D.transform;
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

    #region Damage

    public void TakeDamage(int damage)
    {
        _health -= damage;

    }

    /*    private void OnTriggerEnter2D(Collider2D collision)
        {
            transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)), transform.localScale.y);

            if(collision.gameObject.TryGetComponent(out ITakeDamage takeDamage))
            {
                takeDamage.TakeDamage(_damage);
            }
        }*/

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (_viewDistance - 2) * transform.localScale.x * transform.right);

        Gizmos.DrawWireSphere(transform.position, _distanceToStopping);
    }
}
