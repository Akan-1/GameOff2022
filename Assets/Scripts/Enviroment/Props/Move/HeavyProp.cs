using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyProp : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private Transform _transform;
    private bool _isFollow;

    private PlayerObjectMover _playerObjectMover;

    [SerializeField][Min(0)] private float _playerOffsetX;
    [SerializeField] private float _playerSpeedOnDrag;

    private bool _isCanConnect = true;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (_isFollow)
        {
            Drag();

            bool isRotate = Mathf.Abs(Mathf.RoundToInt(_rigidbody2D.angularVelocity)) > _playerObjectMover.AllowableChangeTransformAngularVelocity;
            bool isPlayerPositionYChange = Mathf.Abs(Mathf.RoundToInt(_playerObjectMover.PlayerController2D.Rigibody2D.velocity.y)) > _playerObjectMover.AllowableChangePlayerVelocityY;
            bool isUnconnectKeyUnpressed = Input.GetKeyUp(KeyCode.F);

            if (isRotate || isPlayerPositionYChange || isUnconnectKeyUnpressed)
            {
                Unconnect();
            }
        }
    }

    #region Connect
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerObjectMover playerObjectMover))
        {
            bool isConnectKeyPressed = Input.GetKeyDown(KeyCode.F);
            bool isPlayerLowerThanProp = playerObjectMover.transform.position.y < transform.position.y;

            if (isConnectKeyPressed && isPlayerLowerThanProp)
            {
                ConnectToPlayer(playerObjectMover);
            }
        }
    }

    public void ConnectToPlayer(PlayerObjectMover playerObjectMover)
    {
        if (_isCanConnect)
        {
            _playerObjectMover = playerObjectMover;
            PlayerController2d playerController = _playerObjectMover.PlayerController2D;

            _rigidbody2D.sharedMaterial = playerController.Rigibody2D.sharedMaterial;

            playerController.IsCanFlip = false;
            playerController.Speed = _playerSpeedOnDrag;


            MovePlayerToCapturePosition();
            _isFollow = true;

            _isCanConnect = false;
        }
    }

    private void MovePlayerToCapturePosition()
    {
        Transform player = _playerObjectMover.transform;
        int playerScaleMultiplier = _playerObjectMover.transform.localScale.x > 0 ? 1 : -1;

        player.position = new Vector3(player.position.x + (_playerOffsetX * playerScaleMultiplier), player.position.y, player.position.z);
    }

    #endregion

    #region Unconnect

    public void Unconnect()
    {
        if (!_isCanConnect && _isFollow)
        {
            PlayerController2d playerController = _playerObjectMover.PlayerController2D;

            _rigidbody2D.sharedMaterial = null;

            playerController.IsCanFlip = true;
            playerController.Speed = playerController.DefaultSpeed;

            _isFollow = false;
            _isCanConnect = true;
        }
    }

    #endregion


    #region Movement

    private void Drag()
    {
        float xVelocity = _playerObjectMover.PlayerController2D.Rigibody2D.velocity.x;
        float yVelocity = _rigidbody2D.velocity.y;
        _rigidbody2D.velocity = new Vector2(xVelocity, yVelocity);
    }

    #endregion

}
