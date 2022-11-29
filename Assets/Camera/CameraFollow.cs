using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _transfrom;
    private Transform _target;
    private float _zPosition;

    [SerializeField] private float _speed;
    [SerializeField] private Vector2 _clampsWidth;
    [SerializeField] private Vector2 _clampsHeight;
    [SerializeField] private Vector3 _offset;

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
}
