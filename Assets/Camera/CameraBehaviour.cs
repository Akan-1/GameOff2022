using System.Collections;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Transform _transfrom;
    private Transform _target;
    private float _zPosition;

    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _clampsWidth;
    [SerializeField] private Vector2 _clampsHeight;
    [SerializeField] private Vector3 _offset;

    [Header("Shake")]
    [SerializeField] private float _shakeDuration;
    [SerializeField] private float _shakeStrenght;
    [SerializeField] private int _shakeCount;
    private IEnumerator _shake;

    public Vector2 Offset => _offset;
    private void Awake()
    {
        _transfrom = GetComponent<Transform>();
        _zPosition = _transfrom.position.z;
    }

    public void StartFollowTo(Transform target)
    {
        _target = target;
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            float xLocalScaleMultiplier = _target.localScale.x > 0 ? -1 : 1;
            float xPosition = Mathf.Clamp(_target.transform.position.x + _offset.x * xLocalScaleMultiplier, _clampsWidth.x, _clampsWidth.y) ;
            float yPosition = Mathf.Clamp(_target.transform.position.y + _offset.y, _clampsHeight.x, _clampsHeight.y);
            Vector3 followVector = new Vector3(xPosition, yPosition, _zPosition + _offset.z);
            _transfrom.position = Vector3.Lerp(_transfrom.position, followVector, _speed * Time.fixedDeltaTime);
        } else
        {
            StartFollowTo(CharacterSwapper.Instance.CurrentPlayerController2D?.transform);

        }
    }

    #region Shake 

    public void BeginShake()
    {
        if (_shake != null)
        {
            StopCoroutine(_shake);
        }

        _shake = Shake();
        StartCoroutine(_shake);
    }

    private IEnumerator Shake()
    {
        float timeOfOneShake = _shakeDuration / _shakeCount;

        for (int i = 1; i < _shakeCount + 1; i++)
        {
            transform.position = GetNewShakePosition(i);
            yield return new WaitForSeconds(timeOfOneShake);
        }

    }

    private Vector3 GetNewShakePosition(int shakeNumber)
    {
        PlayerController2d _currentPlayer = CharacterSwapper.Instance.CurrentPlayerController2D;

        float xLocalScaleMultiplier = _target.localScale.x > 0 ? -1 : 1;

        float newXAdditionalPosition = Random.Range(-(_shakeStrenght / shakeNumber), _shakeStrenght / shakeNumber);
        float newYAdditionalPosition = Random.Range(-(_shakeStrenght / shakeNumber), _shakeStrenght / shakeNumber);

        float XPosition = Mathf.Clamp(_currentPlayer.transform.position.x + Offset.x * xLocalScaleMultiplier + newXAdditionalPosition, _clampsWidth.x, _clampsWidth.y);
        float YPosition = Mathf.Clamp(_currentPlayer.transform.position.y - Offset.y + newYAdditionalPosition, _clampsHeight.x, _clampsHeight.y);
        Vector3 newPosition = new Vector3(XPosition, YPosition, transform.position.z);
        return newPosition;
    }

    #endregion
}
