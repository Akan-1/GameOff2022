using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferObject : MonoBehaviour
{
    private Transform _transform;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _currentTime;

    [SerializeField] private Vector3 newPosition;
    [SerializeField] private AnimationCurve _speedOnActivate;
    [SerializeField] private AnimationCurve _speedOnDisactivate;
    private AnimationCurve _speed;
    private IEnumerator _transfer;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _startPosition = _transform.position;
    }

    #region Transfer
    public void TransferToEndPoint()
    {
        _speed = _speedOnActivate;
        StartTransferTo(_transform.position + newPosition);
    }

    public void TransferToStartPoint()
    {
        _speed = _speedOnDisactivate;
        StartTransferTo(_startPosition);
    }

    private void StartTransferTo(Vector3 target)
    {
        StopTransfer();
        _transfer = TransferTo(target);
        StartCoroutine(_transfer);
    }

    private void StopTransfer()
    {
        if (_transfer != null)
        {
            StopCoroutine(_transfer);
        }
    }

    private IEnumerator TransferTo(Vector3 target)
    {
        _currentTime = 0;

        while (_transform.position != _targetPosition)
        {
            _transform.position = Vector3.Lerp(_transform.position, target, _speed.Evaluate(_currentTime));
            yield return new WaitForSeconds(Time.deltaTime);

            _currentTime += Time.deltaTime;
        }
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Box")
            || collision.gameObject.CompareTag("HeavyBox")
            || collision.gameObject.CompareTag("Tomas")
            || collision.gameObject.CompareTag("Alice"))
        {
            collision.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Box")
            || collision.gameObject.CompareTag("HeavyBox")
            || collision.gameObject.CompareTag("Tomas")
            || collision.gameObject.CompareTag("Alice"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + newPosition, 1);
    }
}
