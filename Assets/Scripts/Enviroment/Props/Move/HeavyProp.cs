using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyProp : MonoBehaviour
{
    private Rigidbody2D _rigibody2D;
    private PlayerController2d _playerController2D;
    private Transform _startParent;
    private Vector3 _startScale;
    private bool _isFollow;

    [SerializeField] private float _playerSpeedOnDrag;


    private void Start()
    {
        _rigibody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_isFollow)
        {
            _rigibody2D.velocity = _playerController2D.Rigibody2D.velocity;
        }
    }

    public void ConnectToPlayerController(PlayerController2d _playerController)
    {
        _isFollow = true;
        _playerController2D = _playerController;
        _playerController.IsCanFlip = false;
        _playerController.Speed = _playerSpeedOnDrag;
    }

    public void Unconnect()
    {
        _isFollow = false;
        _playerController2D.IsCanFlip = true;
        _playerController2D.Speed = _playerController2D.DefaultSpeed;
    }

}
